using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners
{
	public interface NetwonRaphsonSystemEventsListener
	{
		void onInitializationCompleted (List<NodeForNetwonRaphsonSystem> nodes, 
		                                List<EdgeForNetwonRaphsonSystem> edges, 
		                                Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> nodesEnumeration);

		void onRepeatMutateUntilStarted ();

		void onComputationShouldBeStoppedDueTo (UntilConditionAbstract condition);

		void onUnknownVectorAtPreviousStepComputed (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep);

		void onFvectorAtPreviousStepComputed (
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtPreviousStep);

		void onMatricesFixedIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep);

		void onCoefficientsVectorAtCurrentStepComputed (
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep);

		void onNegativeUnknownsFixed (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep);

		void onMutateStepStarted (int? iterationNumber);

		void onAmatrixComputed (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix);

		void onJacobianMatrixComputed (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrix);

		void onKvectorComputed (Vector<EdgeForNetwonRaphsonSystem> Kvector);

		void onUnknownVectorAtCurrentStepComputed (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep);

		void onQvectorComputed (Vector<EdgeForNetwonRaphsonSystem> Qvector);

		void onFvectorAtCurrentStepComputed (Vector<EdgeForNetwonRaphsonSystem> Fvector);

		void onUnknownWithDimensionReverted (Vector<NodeForNetwonRaphsonSystem> unknownVector);

		void onRepeatMutateUntilEnded (OneStepMutationResults result);		

		void onMutateStepCompleted (OneStepMutationResults result);
	}
}

