using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners
{
	public class NetwonRaphsonSystemEventsListenerNullObject : 
		NetwonRaphsonSystemEventsListener
	{
		public NetwonRaphsonSystemEventsListenerNullObject ()
		{
		}
		#region NetwonRaphsonSystemEventsListener implementation
		public void onInitializationCompleted (System.Collections.Generic.List<NodeForNetwonRaphsonSystem> nodes, System.Collections.Generic.List<EdgeForNetwonRaphsonSystem> edges, Lazy<System.Collections.Generic.Dictionary<NodeForNetwonRaphsonSystem, int>> nodesEnumeration)
		{
		
		}

		public void onRepeatMutateUntilStarted ()
		{
		
		}

		public void onComputationShouldBeStoppedDueTo (UntilConditionAbstract condition)
		{
		
		}

		public void onUnknownVectorAtPreviousStepComputed (it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep)
		{
			
		}

		public void onFvectorAtPreviousStepComputed (it.unifi.dsi.stlab.math.algebra.Vector<EdgeForNetwonRaphsonSystem> FvectorAtPreviousStep)
		{
			
		}

		public void onMatricesFixedIfSupplyGadgetsPresent (it.unifi.dsi.stlab.math.algebra.Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, it.unifi.dsi.stlab.math.algebra.Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep)
		{
			
		}

		public void onCoefficientsVectorAtCurrentStepComputed (it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep)
		{
			
		}

		public void onNegativeUnknownsFixed (it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			
		}

		public void onMutateStepStarted (int? iterationNumber)
		{
			
		}

		public void onAmatrixComputed (it.unifi.dsi.stlab.math.algebra.Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix)
		{
			
		}

		public void onJacobianMatrixComputed (it.unifi.dsi.stlab.math.algebra.Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrix)
		{
			
		}

		public void onKvectorComputed (it.unifi.dsi.stlab.math.algebra.Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			
		}

		public void onUnknownVectorAtCurrentStepComputed (it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			
		}

		public void onQvectorComputed (it.unifi.dsi.stlab.math.algebra.Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			
		}

		public void onFvectorAtCurrentStepComputed (it.unifi.dsi.stlab.math.algebra.Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			
		}

		public void onUnknownWithDimensionReverted (
			Dictionary<NodeForNetwonRaphsonSystem, int> nodesEnumeration,
			it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> unknownVector)
		{
			
		}

		public void onRepeatMutateUntilEnded (OneStepMutationResults result)
		{
		
		}

		public void onMutateStepCompleted (OneStepMutationResults result)
		{
		
		}
		#endregion


	}
}

