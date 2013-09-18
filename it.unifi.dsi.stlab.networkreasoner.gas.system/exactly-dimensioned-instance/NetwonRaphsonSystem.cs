using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using log4net;
using log4net.Config;
using System.IO;
using System.Linq;
using it.unifi.dsi.stlab.extensionmethods;
using System.Globalization;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NetwonRaphsonSystem : NetwonRaphsonSystemInterface
	{
		public override Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		public override Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		public override List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public override List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		public GasFormulaVisitor FormulaVisitor{ get; set; }

		public override Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration { get; set; }

		public  Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> EdgesEnumeration { get; set; }

		public NetwonRaphsonSystem ()
		{
			NodesEnumeration = new Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> (
				() => this.Nodes.enumerate ());

			EdgesEnumeration = new Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> (
				() => this.Edges.enumerate ());

		}

		public override void initializeWith (GasNetwork network)
		{

			this.UnknownVector = new Vector<NodeForNetwonRaphsonSystem> ();
			this.Fvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode =
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			var initialUnknownGuessVector = network.makeInitialGuessForUnknowns ();
			var initialFvalueGuessVector = network.makeInitialGuessForFvector ();

			network.doOnNodes (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aNode => {

				var newtonRaphsonNode = new NodeForNetwonRaphsonSystem ();
				newtonRaphsonNode.initializeWith (aNode);

				// un prelievo positivo implica che stiamo prelevando dalla rete
				// quindi i bilanci dei nodi di supply saranno negativi.

				newtonRaphsonNodesByOriginalNode.Add (aNode, newtonRaphsonNode);

				// here we get the initial unknown guess for the current node
				this.UnknownVector.atPut (newtonRaphsonNode,
				                         initialUnknownGuessVector [aNode]);
			}
			)
			);

			this.Nodes = newtonRaphsonNodesByOriginalNode.Values.ToList ();

			List<EdgeForNetwonRaphsonSystem> collector = 
				new List<EdgeForNetwonRaphsonSystem> ();

			network.doOnEdges (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => {

				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
				aBuilder.customNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

				var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);
				collector.Add (edgeForNetwonRaphsonSystem);

				// here we get the initial F values guess for the current node
				this.Fvector.atPut (edgeForNetwonRaphsonSystem,
				                    initialFvalueGuessVector [anEdge]);
			}
			)
			);

			this.Edges = collector;
		}

		abstract class MutateComputationDriver
		{
			public abstract bool canDoOneMoreStep ();

			public abstract void doOneMoreStep (Action step);
		}

		class MutateComputationDriverDoOneMoreMutation : MutateComputationDriver
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver
			public override bool canDoOneMoreStep ()
			{
				return true;
			}

			public override void doOneMoreStep (Action step)
			{
				step.Invoke ();
			}
			#endregion
		}

		class MutateComputationDriverStopMutate : MutateComputationDriver
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver
			public override bool canDoOneMoreStep ()
			{
				return false;
			}

			public override void doOneMoreStep (Action step)
			{
				// nothing to do with step since we've to interrupt the computation
			}
			#endregion
		}

		public override OneStepMutationResults repeatMutateUntil (
			List<UntilConditionAbstract> untilConditions)
		{
			OneStepMutationResults previousOneStepMutationResults = null;

			OneStepMutationResults currentOneStepMutationResults = 
			new OneStepMutationResults{
				IterationNumber = 0,
				Unknowns = this.UnknownVector
			};

			MutateComputationDriver mutateComputationDriver = 
				new MutateComputationDriverDoOneMoreMutation ();

			while (mutateComputationDriver.canDoOneMoreStep()) {			

				untilConditions.ForEach (condition => {

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

					currentOneStepMutationResults = this.mutate (
						previousOneStepMutationResults.IterationNumber + 1);
				}
				);
			}

			return currentOneStepMutationResults;
		}

		public override bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			bool canContinue = condition.canContinue (
				previousOneStepMutationResults, 
				currentOneStepMutationResults);

			return canContinue == false;
		}

		public override OneStepMutationResults mutateWithoutIterationNumber ()
		{
			return this.mutate (null);
		}

		public override Vector<NodeForNetwonRaphsonSystem> computeUnknownVectorAtPreviousStep ()
		{
			var unknownVectorAtPreviousStep = this.UnknownVector;

			return unknownVectorAtPreviousStep;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep ()
		{
			var FvectorAtPreviousStep = Fvector;

			return FvectorAtPreviousStep;
		}

		public override void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep)
		{
			this.Nodes.ForEach (aNode => {
				aNode.fixMatrixIfYouHaveSupplyGadget (AmatrixAtCurrentStep);
				aNode.fixMatrixIfYouHaveSupplyGadget (JacobianMatrixAtCurrentStep);
			}
			);
		}

		public override Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep ()
		{
			var coefficientsVectorAtCurrentStep = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			this.Nodes.ForEach (aNode => aNode.putYourCoefficientInto (
				coefficientsVectorAtCurrentStep, this.FormulaVisitor)
			);

			return coefficientsVectorAtCurrentStep;
		}

		public override void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => currentValue <= 0 ? 
				random.NextDouble () / 10 : currentValue
			);
		}

		public override void updatePreviousVectorsWithCurrentVectors (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep)
		{
			this.UnknownVector = unknownVectorAtCurrentStep;
			this.Fvector = FvectorAtCurrentStep;
		}

		public override OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int? iterationNumber)
		{
			var result = new OneStepMutationResults ();
			result.Amatrix = AmatrixAtCurrentStep;
			result.Unknowns = unknownVectorAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;
			result.Jacobian = JacobianMatrixAtCurrentStep;
			result.Fvector = FvectorAtCurrentStep;
			if (iterationNumber.HasValue) {
				result.IterationNumber = iterationNumber.Value;
			}

			return result;
		}

		public override OneStepMutationResults mutate (
			Nullable<int> iterationNumber)
		{
			var unknownVectorAtPreviousStep = this.computeUnknownVectorAtPreviousStep ();

			var FvectorAtPreviousStep = this.computeFvectorAtPreviousStep ();

			var KvectorAtCurrentStep = computeKvector (
				unknownVectorAtPreviousStep,
				FvectorAtPreviousStep);

			var AmatrixAtCurrentStep =
				computeAmatrix (KvectorAtCurrentStep);

			var JacobianMatrixAtCurrentStep =
				computeJacobianMatrix (KvectorAtCurrentStep);

			this.fixMatricesIfSupplyGadgetsPresent (AmatrixAtCurrentStep, JacobianMatrixAtCurrentStep);

			var coefficientsVectorAtCurrentStep = this.computeCoefficientsVectorAtCurrentStep ();

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				this.computeUnknowns (
					AmatrixAtCurrentStep, 
					unknownVectorAtPreviousStep, 
					coefficientsVectorAtCurrentStep, 
					JacobianMatrixAtCurrentStep);

			this.fixNegativeUnknowns (unknownVectorAtCurrentStep);

			var QvectorAtCurrentStep = computeQvector (
				unknownVectorAtCurrentStep, 
				KvectorAtCurrentStep);

			var FvectorAtCurrentStep = computeFvector (
				FvectorAtPreviousStep, 
				QvectorAtCurrentStep);

			updatePreviousVectorsWithCurrentVectors (
				unknownVectorAtCurrentStep, FvectorAtCurrentStep);

			var result = computeOneStepMutationResult (AmatrixAtCurrentStep, 
			                                           unknownVectorAtCurrentStep, 
			                                           coefficientsVectorAtCurrentStep, 
			                                           QvectorAtCurrentStep, 
			                                           JacobianMatrixAtCurrentStep, 
			                                           FvectorAtCurrentStep, 
			                                           iterationNumber);


			return result;
		}

		public override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.fillAmatrixUsing (
				aMatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

			return aMatrix;
		}

		public override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.fillJacobianMatrixUsing (
				aMatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

			return aMatrix;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putKvalueIntoUsing (
				Kvector, Fvector, unknownVector, this.FormulaVisitor)
			);

			return Kvector;
		}

		public override Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep)
		{
			Vector<NodeForNetwonRaphsonSystem> matrixArightProductUnknownAtPreviousStep = 
				AmatrixAtCurrentStep.rightProduct (unknownVectorAtPreviousStep);

			Vector<NodeForNetwonRaphsonSystem> coefficientVectorForJacobianSystemFactorization = 
				matrixArightProductUnknownAtPreviousStep.minus (coefficientsVectorAtCurrentStep);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorFromJacobianSystemAtCurrentStep =
				jacobianMatrixAtCurrentStep.SolveWithGivenEnumerations (
					this.NodesEnumeration.Value,
					this.NodesEnumeration.Value,
					coefficientVectorForJacobianSystemFactorization);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				unknownVectorAtPreviousStep.minus (unknownVectorFromJacobianSystemAtCurrentStep);

			return unknownVectorAtCurrentStep;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putQvalueIntoUsing (
				Qvector, Kvector, unknownVector, this.FormulaVisitor)
			);

			return Qvector;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putNewFvalueIntoUsing (
				newFvector, Qvector, Fvector, this.FormulaVisitor)
			);

			return newFvector;
		}

		public override Vector<NodeForNetwonRaphsonSystem> denormalizeUnknowns ()
		{
			
			// here we're assuming that the initial pressure vector for unknowns 
			// is given in relative way, otherwise the following transformation
			// isn't correct because mix values with different measure unit.
			this.UnknownVector.updateEach (
				(aNode, absolutePressure) => 
				aNode.relativePressureOf (absolutePressure, this.FormulaVisitor)
			);

		
			return this.UnknownVector;
		}

	}
}

