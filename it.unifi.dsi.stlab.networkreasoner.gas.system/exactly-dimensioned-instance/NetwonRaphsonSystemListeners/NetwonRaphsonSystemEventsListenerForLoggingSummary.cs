using System;
using log4net;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.utilities.value_holders;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners
{
	public class NetwonRaphsonSystemEventsListenerForLoggingSummary :
		NetwonRaphsonSystemEventsListener
	{
		Lazy<System.Collections.Generic.Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration {
			get;
			set;
		}

		DateTime StartTime{ get; set; }

		DateTime EndTime{ get; set; }

		public ILog Log{ get; set; }

		System.Collections.Generic.List<EdgeForNetwonRaphsonSystem> Edges {
			get;
			set;
		}

		public void onMutateStepStarted (int? iterationNumber)
		{
//			if (iterationNumber.HasValue) {
//				this.Log.InfoFormat ("-------------------- Iteration {0} ---------------------", 
//				                    iterationNumber.Value);
//			}
		}

		public void onRepeatMutateUntilStarted ()
		{
			this.StartTime = DateTime.Now;
		}

		public void onMutateStepCompleted (OneStepMutationResults result)
		{
			result.Unknowns.forComputationAmong (
				this.NodesEnumeration.Value, new ValueHolderNoInfoShouldBeRequested<Double> ()).stringRepresentation (
				representation => this.Log.InfoFormat (
				"{0}", representation.Substring (representation.IndexOf ('\n') + 1)
			)
			);
		}

		public void onRepeatMutateUntilEnded (OneStepMutationResults result)
		{
			this.EndTime = DateTime.Now;

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				result.Qvector, (edgeRepresentation, QvalueRepresentation) => 
				this.Log.InfoFormat ("Q value of {0} at current step: {1}", 
			                    edgeRepresentation, QvalueRepresentation)
			)
			);

			this.Log.InfoFormat ("Elapsed time of system solution is {0} seconds", 
			                    (this.EndTime.Ticks - this.StartTime.Ticks) / 1e+7);
		}

		public void onInitializationCompleted (
			System.Collections.Generic.List<NodeForNetwonRaphsonSystem> nodes, 
			System.Collections.Generic.List<EdgeForNetwonRaphsonSystem> edges, 
			Lazy<System.Collections.Generic.Dictionary<NodeForNetwonRaphsonSystem, int>> nodesEnumeration)
		{
			this.NodesEnumeration = nodesEnumeration;
			this.Edges = edges;
		}

		public void onUnknownWithDimensionReverted (
			Dictionary<NodeForNetwonRaphsonSystem, int> nodesEnumeration,
			Vector<NodeForNetwonRaphsonSystem> unknownVector)
		{
			unknownVector.forComputationAmong (
				nodesEnumeration, new ValueHolderNoInfoShouldBeRequested<Double> ()).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Relative Unknowns vector at current step: {0}", representation)
			);
		}


		#region NetwonRaphsonSystemEventsListener implementation

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

		#endregion


	}
}

