using System;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public interface NetwonRaphsonSystemInterface
	{
		Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration { get; set; }

		void initializeWith (GasNetwork network);

		OneStepMutationResults repeatMutateUntil (
			List<UntilConditionAbstract> untilConditions);

		bool CheckUntilCondition (Func<bool> condition);

		OneStepMutationResults mutateWithoutIterationNumber ();

		Vector<NodeForNetwonRaphsonSystem> computeUnknownsAtPreviousStep ();

		Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep ();

		void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep);

		Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep ();

		void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep);

		void updatePreviousVectorsWithCurrentVectors (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep);

		OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int? iterationNumber);

		OneStepMutationResults mutate (
			Nullable<int> iterationNumber);

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep);

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep);

		Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector);

		Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep);

		Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector);

		Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector);

		Vector<NodeForNetwonRaphsonSystem> denormalizeUnknowns ();
	}
}

