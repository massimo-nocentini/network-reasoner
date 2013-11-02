using System;
using log4net;
using System.Collections.Generic;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners
{
	public class NetwonRaphsonSystemEventsListenerForLogging :
		NetwonRaphsonSystemEventsListener
	{
		public 	ILog Log {
			get;
			set;
		}

		Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration {
			get;
			set;
		}

		List<EdgeForNetwonRaphsonSystem> Edges {
			get;
			set;
		}

		#region NetwonRaphsonSystemEventsListener implementation

		public void onMutateStepCompleted (OneStepMutationResults result)
		{
		
		}

		public void onInitializationCompleted (
			System.Collections.Generic.List<NodeForNetwonRaphsonSystem> nodes, 
			System.Collections.Generic.List<EdgeForNetwonRaphsonSystem> edges,
			Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> nodesEnumeration)
		{
			this.NodesEnumeration = nodesEnumeration;
			this.Edges = edges;

			this.Log.Info ("================================================================");
			this.Log.Info ("Start of a new mutation step");

			this.Log.Info ("The following nodes enumeration is used throughout the system computation:");
			foreach (var pair in this.NodesEnumeration.Value) {
				this.Log.InfoFormat ("Node: {0} -> Index: {1}", 
				                     pair.Key.Identifier, pair.Value);
			}

//			this.Log.Info ("The following edges enumeration is used throughout the system computation:");
//			foreach (var pair in this.DecoredSystemImplementation.EdgesEnumeration.Value) {
//				this.Log.InfoFormat ("(StartNode, EndNode): ({0},{1}) -> Index: {2}", 
//				                     pair.Key.StartNode.Identifier, 
//				                     pair.Key.EndNode.Identifier, 
//				                     pair.Value);
//			}
		}

		public void onRepeatMutateUntilStarted ()
		{
			this.Log.Info ("***********************************************************");
			this.Log.Info ("*  Start of a new computation driven by until conditions  *");
			this.Log.Info ("***********************************************************");
		}

		public void onComputationShouldBeStoppedDueTo (UntilConditionAbstract condition)
		{
			string stoppingCauseDescription = condition.stoppingCauseDescription ();
			this.Log.InfoFormat ("Computation will be stopped due to reason: {0}", 
				                    stoppingCauseDescription);
		}

		public void onUnknownVectorAtPreviousStepComputed (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep)
		{
			unknownVectorAtPreviousStep.forComputationAmong (
				this.NodesEnumeration.Value, new ValueHolderNoInfoShouldBeRequested<Double> ()).stringRepresentation (
				representation => this.Log.InfoFormat (
				"Relative Unknowns at previous step: {0}", representation)
			);
		}

		public void onFvectorAtPreviousStepComputed (
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtPreviousStep)
		{
			
			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				FvectorAtPreviousStep, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at previous step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);
		}

		public void onMatricesFixedIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep)
		{
			AmatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).stringRepresentation (
				representation => this.Log.InfoFormat ("A matrix at current step after supply node fix it:\n{0}", representation));

			JacobianMatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).stringRepresentation (
				representation => this.Log.InfoFormat ("Jacobian matrix at current step after supply node fix it:\n{0}", representation));
		}

		public void onCoefficientsVectorAtCurrentStepComputed (
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep)
		{
			coefficientsVectorAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, new ValueHolderNoInfoShouldBeRequested<Double> ()).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Coefficients vector at current step: {0}", representation)
			);
		}

		public void onNegativeUnknownsFixed (Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			unknownVectorAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, new ValueHolderNoInfoShouldBeRequested<Double> ()).stringRepresentation (
				representation => this.Log.InfoFormat (
				"Adimensional unknowns vector at current step after fix negative entries: {0}", representation)
			);
		}

		public void onMutateStepStarted (int? iterationNumber)
		{
			if (iterationNumber.HasValue) {
				this.Log.InfoFormat ("-------------------- Iteration {0} ---------------------", 
				                    iterationNumber.Value);
			}
		}

		public void onAmatrixComputed (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix)
		{
			Amatrix.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"A matrix at current step before supply node fix it:\n{0}", representation)
			);

		}

		public void onJacobianMatrixComputed (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrix)
		{
			JacobianMatrix.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Jacobian matrix at current step before supply node fix it:\n{0}", representation)
			);

		}

		public void onKvectorComputed (Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				Kvector, (edgeRepresentation, KvalueRepresentation) => 
				this.Log.InfoFormat ("K value of {0} at current step: {1}", 
			                    edgeRepresentation, KvalueRepresentation)
			)
			);

		}

		public void onUnknownVectorAtCurrentStepComputed (Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			unknownVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, new ValueHolderNoInfoShouldBeRequested<Double> ()).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Adimensional unknowns vector at current step before fix negative entries: {0}", representation)
			);

		}

		public void onQvectorComputed (Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				Qvector, (edgeRepresentation, QvalueRepresentation) => 
				this.Log.InfoFormat ("Q value of {0} at current step: {1}", 
			                    edgeRepresentation, QvalueRepresentation)
			)
			);

		}

		public void onFvectorAtCurrentStepComputed (Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				Fvector, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at current step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);

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

		public void onRepeatMutateUntilEnded (OneStepMutationResults result)
		{

		}
		#endregion



			





	}
}

