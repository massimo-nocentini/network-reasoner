using System;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public abstract class NetwonRaphsonSystemInterface
	{
		public abstract Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		public abstract Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		public abstract List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public abstract List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		public abstract Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration { get; set; }

		public abstract void initializeWith (GasNetwork network);

		public abstract OneStepMutationResults repeatMutateUntil (
			List<UntilConditionAbstract> untilConditions);

		public abstract bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults);

		public abstract OneStepMutationResults mutateWithoutIterationNumber ();

		public abstract Vector<NodeForNetwonRaphsonSystem> computeUnknownVectorAtPreviousStep ();

		public abstract Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep ();

		public abstract void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep);

		public abstract Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep ();

		public abstract void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep);

		public abstract void updatePreviousVectorsWithCurrentVectors (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep);

		public abstract OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int? iterationNumber);

		public abstract OneStepMutationResults mutate (
			Nullable<int> iterationNumber);

		public abstract Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep);

		public abstract Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep);

		public abstract Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector);

		public abstract Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep);

		public abstract Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector);

		public abstract Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector);

		public abstract Vector<NodeForNetwonRaphsonSystem> denormalizeUnknowns ();


	}
}

