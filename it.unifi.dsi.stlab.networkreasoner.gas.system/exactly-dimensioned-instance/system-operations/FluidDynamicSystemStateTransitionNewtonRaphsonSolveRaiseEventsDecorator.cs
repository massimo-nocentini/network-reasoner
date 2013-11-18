using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionNewtonRaphsonSolveRaiseEventsDecorator
		: FluidDynamicSystemStateTransitionNewtonRaphsonSolve
	{
		public NetwonRaphsonSystemEventsListener EventsListener{ get; set; }

		public override FluidDynamicSystemStateAbstract forUnsolvedSystemState (
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			EventsListener.onRepeatMutateUntilStarted ();

			var abstractState = base.forUnsolvedSystemState (fluidDynamicSystemStateUnsolved);

			if (abstractState is FluidDynamicSystemStateMathematicallySolved) {

				var mathematicallySolvedState = abstractState as FluidDynamicSystemStateMathematicallySolved;

				EventsListener.onRepeatMutateUntilEnded (mathematicallySolvedState.MutationResult);

			} else {
				// TODO this else branch has to be removed at the end of the refactoring
				throw new Exception ("Massimo's mistake: check the creation of mathematically solved state after Newton-Raphson solve transition");
			}

			return abstractState;
		}

		protected override bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			var computationShouldBeStopped = base.decideIfComputationShouldBeStopped (
				condition, previousOneStepMutationResults, currentOneStepMutationResults);

			if (computationShouldBeStopped) {
				EventsListener.onComputationShouldBeStoppedDueTo (condition);
			}

			return computationShouldBeStopped;
		}

		protected override OneStepMutationResults mutate (
			OneStepMutationResults previousStepMutationResults,
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			EventsListener.onMutateStepStarted (previousStepMutationResults.IterationNumber);

			var result = base.mutate (previousStepMutationResults, fluidDynamicSystemStateUnsolved);

			EventsListener.onMutateStepCompleted (result);
		
			return result;
		}

		protected override DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> 
			computeUnknownVectorAtPreviousStep (OneStepMutationResults previousStepMutationResults)
		{
			var unknownVectorAtPreviousStep = base.computeUnknownVectorAtPreviousStep (
				previousStepMutationResults);
		
			EventsListener.onUnknownVectorAtPreviousStepComputed (
				unknownVectorAtPreviousStep.WrappedObject);

			return unknownVectorAtPreviousStep;
		}

		protected override Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep (
			OneStepMutationResults previousStepMutationResults)
		{
			var FvectorAtPreviousStep = base.computeFvectorAtPreviousStep (previousStepMutationResults);

			EventsListener.onFvectorAtPreviousStepComputed (
				FvectorAtPreviousStep);

			return FvectorAtPreviousStep;
		}

		protected override Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			var Kvector = base.computeKvector (unknownVector, Fvector, edges);
			
			this.EventsListener.onKvectorComputed (Kvector);
		
			return Kvector;
		}

		protected override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			                List<EdgeForNetwonRaphsonSystem> edges)
		{
			var Amatrix = base.computeAmatrix (kvectorAtCurrentStep, edges);
			
			EventsListener.onAmatrixComputed (Amatrix);

			return Amatrix;
		}

		protected override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			                       List<EdgeForNetwonRaphsonSystem> edges)
		{
			var JacobianMatrix = base.computeJacobianMatrix (kvectorAtCurrentStep, edges);
			
			this.EventsListener.onJacobianMatrixComputed (JacobianMatrix);

			return JacobianMatrix;
		}

		protected override void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			List<NodeForNetwonRaphsonSystem> nodes)
		{
			base.fixMatricesIfSupplyGadgetsPresent (AmatrixAtCurrentStep, JacobianMatrixAtCurrentStep, nodes);
			
			EventsListener.onMatricesFixedIfSupplyGadgetsPresent (
				AmatrixAtCurrentStep, 
				JacobianMatrixAtCurrentStep);
		}

		protected override Vector<NodeForNetwonRaphsonSystem> 
			computeCoefficientsVectorAtCurrentStep (List<NodeForNetwonRaphsonSystem> nodes)
		{
			var coefficientsVectorAtCurrentStep = base.computeCoefficientsVectorAtCurrentStep (nodes);
			
			this.EventsListener.onCoefficientsVectorAtCurrentStepComputed (
				coefficientsVectorAtCurrentStep);

			return coefficientsVectorAtCurrentStep;
		}

		protected override DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep, 
			Dictionary<NodeForNetwonRaphsonSystem, int> nodesEnumeration)
		{
			var result = base.computeUnknowns (
				AmatrixAtCurrentStep, 
				unknownVectorAtPreviousStep, 
				coefficientsVectorAtCurrentStep, 
				jacobianMatrixAtCurrentStep, 
				nodesEnumeration);

			var unknownVectorAtCurrentStep = result.WrappedObject;
			EventsListener.onUnknownVectorAtCurrentStepComputed (unknownVectorAtCurrentStep);

			return result;
		}

		protected override void fixNegativeUnknowns (Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			base.fixNegativeUnknowns (unknownVectorAtCurrentStep);
			EventsListener.onNegativeUnknownsFixed (unknownVectorAtCurrentStep);
		}

		protected override Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			var Qvector = base.computeQvector (unknownVector, Kvector, edges);
			
			EventsListener.onQvectorComputed (Qvector);

			return Qvector;
		}

		protected override Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			List<EdgeForNetwonRaphsonSystem> edges)
		{
			var newFvector = base.computeFvector (Fvector, Qvector, edges);

			EventsListener.onFvectorAtCurrentStepComputed (newFvector);

			return newFvector;
		}
	}
}

