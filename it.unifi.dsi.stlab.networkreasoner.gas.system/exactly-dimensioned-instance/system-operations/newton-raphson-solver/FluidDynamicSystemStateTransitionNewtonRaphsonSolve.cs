using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionNewtonRaphsonSolve
		: FluidDynamicSystemStateTransition
	{
		public List<UntilConditionAbstract> UntilConditions{ get; set; }
		//public Func<Vector<NodeForNetwonRaphsonSystem>, Vector<NodeForNetwonRaphsonSystem>> FromDimensionalToAdimensionalTranslator{ get; set; }
		public GasFormulaVisitor FormulaVisitor{ get; set; }


		#region FluidDynamicSystemStateTransition implementation

		public FluidDynamicSystemStateAbstract forBareSystemState (
			FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			throw new Exception ("A system in the bare state cannot be solved, initialize it before and try again");
		}

		public virtual FluidDynamicSystemStateAbstract forUnsolvedSystemState (
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			var mathematicallySolvedState = new FluidDynamicSystemStateMathematicallySolved ();

			var startTimeStamp = DateTime.UtcNow;

			OneStepMutationResults previousOneStepMutationResults = null;

			OneStepMutationResults currentOneStepMutationResults = 
				new OneStepMutationResults {
					IterationNumber = 0,
					Unknowns = fluidDynamicSystemStateUnsolved.InitialUnknownVector,
					Fvector = fluidDynamicSystemStateUnsolved.InitialFvector
				};

			MutateComputationDriver mutateComputationDriver = 
				new MutateComputationDriverDoOneMoreMutation ();

			while (mutateComputationDriver.canDoOneMoreStep ()) {			

				UntilConditions.ForEach (condition => {

					if (this.decideIfComputationShouldBeStopped (condition,
						    previousOneStepMutationResults, 
						    currentOneStepMutationResults)) {

						mutateComputationDriver = new MutateComputationDriverStopMutate ();
					}
				}
				);

				mutateComputationDriver.doOneMoreStep (
					step: () => {

						previousOneStepMutationResults = currentOneStepMutationResults;

						previousOneStepMutationResults.IterationNumber += 1;

						currentOneStepMutationResults = this.mutate (
							previousOneStepMutationResults,
							fluidDynamicSystemStateUnsolved);
					}
				);
			}

			currentOneStepMutationResults.VelocityVector = computeVelocityVector (
				currentOneStepMutationResults.Unknowns, 
				currentOneStepMutationResults.Qvector, 
				fluidDynamicSystemStateUnsolved.Nodes,
				fluidDynamicSystemStateUnsolved.Edges);

			currentOneStepMutationResults.ComputationEndTimestamp = DateTime.UtcNow;
			currentOneStepMutationResults.ComputationStartTimestamp = startTimeStamp;
			mathematicallySolvedState.MutationResult = currentOneStepMutationResults;
			mathematicallySolvedState.SolvedBy = this;

			return mathematicallySolvedState;		
		}

		public FluidDynamicSystemStateAbstract forMathematicallySolvedState (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			// simply return the given state since if it is already mathematically solved we've no work to do here
			return fluidDynamicSystemStateMathematicallySolved;
		}

		public FluidDynamicSystemStateAbstract forNegativeLoadsCorrectedState (
			FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			// simply return the given state since if it is already checked for negative pressures, someone mathematically solved it already.
			return fluidDynamicSystemStateNegativeLoadsCorrected;
		}

		#endregion

		protected virtual bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			bool until = condition.canContinue (
				             previousOneStepMutationResults, 
				             currentOneStepMutationResults);

			bool computationShouldBeStopped = until == false;

			return computationShouldBeStopped;
		}

		protected virtual OneStepMutationResults mutate (
			OneStepMutationResults previousStepMutationResults,
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			var mutateStartTimestamp = DateTime.UtcNow;

			var unknownVectorAtPreviousStep = computeUnknownVectorAtPreviousStep (
				                                  previousStepMutationResults);

			var adimensionalUnknownVectorAtPreviousStep = makeAdimensionalUnknowns (
				                                              unknownVectorAtPreviousStep);

			var FvectorAtPreviousStep = computeFvectorAtPreviousStep (
				                            previousStepMutationResults);

			var KvectorAtCurrentStep = computeKvector (
				                           adimensionalUnknownVectorAtPreviousStep.WrappedObject,
				                           FvectorAtPreviousStep,
				                           fluidDynamicSystemStateUnsolved.Edges);

			var AmatrixAtCurrentStep = computeAmatrix (
				                           KvectorAtCurrentStep,
				                           fluidDynamicSystemStateUnsolved.Edges);

			var JacobianMatrixAtCurrentStep = computeJacobianMatrix (
				                                  KvectorAtCurrentStep,
				                                  fluidDynamicSystemStateUnsolved.Edges);

			fixMatricesIfSupplyGadgetsPresent (
				AmatrixAtCurrentStep, 
				JacobianMatrixAtCurrentStep,
				fluidDynamicSystemStateUnsolved.Nodes);

			var coefficientsVectorAtCurrentStep = computeCoefficientsVectorAtCurrentStep (
				                                      fluidDynamicSystemStateUnsolved.Nodes,
				                                      previousStepMutationResults.Qvector);

			DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> unknownVectorAtCurrentStep = 
				computeUnknowns (
					AmatrixAtCurrentStep, 
					adimensionalUnknownVectorAtPreviousStep.WrappedObject, 
					coefficientsVectorAtCurrentStep, 
					JacobianMatrixAtCurrentStep,
					fluidDynamicSystemStateUnsolved.NodesEnumeration);

			fixNegativeUnknowns (unknownVectorAtCurrentStep.WrappedObject);

			fixLesserUnknownsForAntecedentsInPressureRegulations (
				unknownVectorAtCurrentStep.WrappedObject);

			var QvectorAtCurrentStep = computeQvector (
				                           unknownVectorAtCurrentStep.WrappedObject, 
				                           KvectorAtCurrentStep,
				                           fluidDynamicSystemStateUnsolved.Edges);

			var FvectorAtCurrentStep = computeFvector (
				                           FvectorAtPreviousStep, 
				                           QvectorAtCurrentStep,
				                           fluidDynamicSystemStateUnsolved.Edges);

			var result = computeOneStepMutationResult (AmatrixAtCurrentStep, 
				             unknownVectorAtCurrentStep, 
				             coefficientsVectorAtCurrentStep, 
				             QvectorAtCurrentStep, 
				             JacobianMatrixAtCurrentStep, 
				             FvectorAtCurrentStep, 
				             previousStepMutationResults.IterationNumber,
				             mutateStartTimestamp,
				             fluidDynamicSystemStateUnsolved);

			return result;
		}

		#region ``compute'' methods

		protected virtual DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> makeAdimensionalUnknowns (
			DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> unknownVectorAtPreviousStep)
		{
			// we perform the adimensional translation just to ensure that we're 
			// working with adimensional objects, since the translator raise an exception
			// if it is called.
			return unknownVectorAtPreviousStep.translateTo (
				new AdimensionalPressures ());
		}

		protected virtual DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> 
			computeUnknownVectorAtPreviousStep (OneStepMutationResults previousStepMutationResults)
		{
			return previousStepMutationResults.Unknowns;
		}

		protected virtual Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep (
			OneStepMutationResults previousStepMutationResults)
		{
			return previousStepMutationResults.Fvector;
		}

		protected virtual void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep,
			List<NodeForNetwonRaphsonSystem> nodes)
		{
			nodes.ForEach (aNode => {
				aNode.fixMatrixIfYouHaveSupplyGadget (AmatrixAtCurrentStep);
				aNode.fixMatrixIfYouHaveSupplyGadget (JacobianMatrixAtCurrentStep);
			}
			);
		}

		protected virtual Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep (
			List<NodeForNetwonRaphsonSystem> nodes,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			var coefficientsVectorAtCurrentStep = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			nodes.ForEach (aNode => aNode.putYourCoefficientInto (
				coefficientsVectorAtCurrentStep, FormulaVisitor, Qvector)
			);

			return coefficientsVectorAtCurrentStep;
		}

		protected virtual void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => currentValue <= 0 ? 
				random.NextDouble () / 10 : currentValue
			);
		}

		protected virtual void fixLesserUnknownsForAntecedentsInPressureRegulations (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => aNode.fixPressureForAntecedentInReduction (
					unknownVectorAtCurrentStep, currentValue)
			);
		}

		public OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int iterationNumber,
			DateTime stepStartTimestamp,
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			var result = new OneStepMutationResults ();
			result.Amatrix = AmatrixAtCurrentStep;
			result.Unknowns = unknownVectorAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;
			result.Jacobian = JacobianMatrixAtCurrentStep;
			result.Fvector = FvectorAtCurrentStep;
			result.IterationNumber = iterationNumber;
			result.StartingUnsolvedState = fluidDynamicSystemStateUnsolved;
			result.UsedFormulae = FormulaVisitor;
			result.ComputationStartTimestamp = stepStartTimestamp;
			result.ComputationEndTimestamp = DateTime.UtcNow;
			return result;
		}

		protected virtual Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep,
		                 List<EdgeForNetwonRaphsonSystem> edges)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			edges.ForEach (anEdge => anEdge.fillAmatrixUsing (
				Amatrix, kvectorAtCurrentStep, FormulaVisitor)
			);

			return Amatrix;
		}

		protected virtual Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep,
		                        List<EdgeForNetwonRaphsonSystem> edges)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			edges.ForEach (anEdge => anEdge.fillJacobianMatrixUsing (
				JacobianMatrix, kvectorAtCurrentStep, FormulaVisitor)
			);

			return JacobianMatrix;
		}

		protected virtual Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector,
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			edges.ForEach (anEdge => anEdge.putKvalueIntoUsing (
				Kvector, Fvector, unknownVector, FormulaVisitor)
			);

			return Kvector;
		}

		protected virtual DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep,
			Dictionary<NodeForNetwonRaphsonSystem, int> nodesEnumeration)
		{
			Vector<NodeForNetwonRaphsonSystem> matrixArightProductUnknownAtPreviousStep = 
				AmatrixAtCurrentStep.rightProduct (unknownVectorAtPreviousStep);

			Vector<NodeForNetwonRaphsonSystem> coefficientVectorForJacobianSystemFactorization = 
				matrixArightProductUnknownAtPreviousStep.minus (coefficientsVectorAtCurrentStep);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorFromJacobianSystemAtCurrentStep =
				jacobianMatrixAtCurrentStep.SolveWithGivenEnumerations (
					nodesEnumeration,
					nodesEnumeration,
					coefficientVectorForJacobianSystemFactorization);

			unknownVectorFromJacobianSystemAtCurrentStep.updateEach (
				(node, currentValue) => currentValue * .75);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				unknownVectorAtPreviousStep.minus (unknownVectorFromJacobianSystemAtCurrentStep);

			return new DimensionalObjectWrapperWithAdimensionalValues<Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = unknownVectorAtCurrentStep
			};
		}

		protected virtual Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector,
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			Vector<EdgeForNetwonRaphsonSystem> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			edges.ForEach (anEdge => anEdge.putQvalueIntoUsing (
				Qvector, Kvector, unknownVector, FormulaVisitor)
			);

			return Qvector;
		}

		protected virtual Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector,
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			Vector<EdgeForNetwonRaphsonSystem> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			edges.ForEach (anEdge => anEdge.putNewFvalueIntoUsing (
				newFvector, Qvector, Fvector, FormulaVisitor)
			);

			return newFvector;
		}

		protected virtual Vector<EdgeForNetwonRaphsonSystem> computeVelocityVector (
			DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> pressuresWrapper, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector,
			List<NodeForNetwonRaphsonSystem> nodes,
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			Vector<EdgeForNetwonRaphsonSystem> velocityVector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			var absolutePressures = pressuresWrapper.translateTo (new AbsolutePressures {
				Nodes = nodes,
				Formulae = FormulaVisitor
			}
			                        ).WrappedObject;

			edges.ForEach (anEdge => anEdge.putVelocityValueIntoUsing (
				velocityVector, absolutePressures, Qvector, FormulaVisitor)
			);

			return velocityVector;
		}

		public FluidDynamicSystemStateTransition clone ()
		{
			throw new System.NotImplementedException ();
		}

		#endregion
	}
}

