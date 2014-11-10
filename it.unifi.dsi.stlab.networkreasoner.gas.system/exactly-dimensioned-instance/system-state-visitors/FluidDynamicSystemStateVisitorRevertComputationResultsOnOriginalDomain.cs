using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.extension_methods;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain
		: FluidDynamicSystemStateVisitorWithSystemName
	{
		public Dictionary<GasEdgeAbstract, double> FlowsByEdges {
			get;
			set;
		}

		public Dictionary<GasEdgeAbstract, double> VelocitiesByEdges {
			get;
			set;
		}

		public Dictionary<GasNodeAbstract, double> PressuresByNodes {
			get;
			set;
		}

		public Dictionary<GasNodeAbstract, double> AlgebraicSumOfFlowsByNodes {
			get;
			set;
		}

		public Dictionary<GasNodeAbstract, StringBuilder> AnomaliesByNodes {
			get;
			set;
		}

		public Dictionary<GasEdgeAbstract, StringBuilder> AnomaliesByEdges {
			get;
			set;
		}

		public TimeSpan? ElapsedTime {
			get;
			set;
		}

		#region FluidDynamicSystemStateVisitorWithSystemName implementation

		public string SystemName{ get; set; }

		public void forBareSystemState (FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			throw new Exception ("It is no possible to map computation result on " +
			"original domain since we're in a bare system state: initialize and " +
			"solve it before doing this mapping"
			);
		}

		public void forUnsolvedSystemState (FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			throw new Exception ("It is no possible to map computation result on " +
			"original domain since we're in an unsolved system state: " +
			"solve it before doing this mapping"
			);
		}

		void findEdgeAnomalies (
			EdgeForNetwonRaphsonSystem anEdge, 
			double Qvalue, 
			double VelocityValue,
			GasEdgeAbstract originalEdge)
		{
			var anomalyFinder = new EdgeAnomalyFinder ();
			anomalyFinder.Flow = Qvalue;
			anomalyFinder.Velocity = VelocityValue;
			anomalyFinder.ReportAnomaly = anAnomaly => {
				if (AnomaliesByEdges.ContainsKey (originalEdge) == false) {
					AnomaliesByEdges.Add (originalEdge, new StringBuilder ());
				}
				AnomaliesByEdges [originalEdge].AppendFormat (
					"Node {0}: {1}.", 
					anEdge.Identifier, 
					anAnomaly);
			};
			originalEdge.accept (anomalyFinder);
		}

		void findNodeAnomalies (
			String nodeIdentifier, 
			GasNodeAbstract originalNode,
			double nodePressure,
			double algebraicSumOfFlows)
		{
			var anomalyFinder = new NodeAnomalyFinder ();
			anomalyFinder.NodePressure = nodePressure;
			anomalyFinder.NodeAlgebraicSumOfFlows = algebraicSumOfFlows;
			anomalyFinder.ReportAnomaly = anAnomaly => {
				if (AnomaliesByNodes.ContainsKey (originalNode) == false) {
					AnomaliesByNodes.Add (originalNode, new StringBuilder ());
				}
				AnomaliesByNodes [originalNode].AppendFormat (
					"Node {0}: {1}.", 
					nodeIdentifier, 
					anAnomaly);
			};
			originalNode.accept (anomalyFinder);
		}

		public void forMathematicallySolvedState (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			PressuresByNodes = new Dictionary<GasNodeAbstract, double> ();
			AlgebraicSumOfFlowsByNodes = new Dictionary<GasNodeAbstract, double> ();
			FlowsByEdges = new Dictionary<GasEdgeAbstract, double> ();
			VelocitiesByEdges = new Dictionary<GasEdgeAbstract, double> ();
			AnomaliesByNodes = new Dictionary<GasNodeAbstract, StringBuilder> ();
			AnomaliesByEdges = new Dictionary<GasEdgeAbstract, StringBuilder> ();

			var mutationResults = fluidDynamicSystemStateMathematicallySolved.MutationResult;
			ElapsedTime = computeElapsedTimeFrom (mutationResults);

			var dimensionalUnknowns = fluidDynamicSystemStateMathematicallySolved.
				MutationResult.makeUnknownsDimensional ().WrappedObject;

			foreach (var originalByComputationNodesPair in 
			         fluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

				PressuresByNodes.Add (originalByComputationNodesPair.Value, 
					dimensionalUnknowns.valueAt (originalByComputationNodesPair.Key));
			
				AlgebraicSumOfFlowsByNodes.Add (originalByComputationNodesPair.Value, 0d);
			}

			foreach (var originalByComputationEdgesPair in
			        fluidDynamicSystemStateMathematicallySolved.MutationResult.StartingUnsolvedState.OriginalEdgesByComputationEdges) {

				var Qvalue = fluidDynamicSystemStateMathematicallySolved.
					MutationResult.Qvector.valueAt (originalByComputationEdgesPair.Key);

				var VelocityValue = fluidDynamicSystemStateMathematicallySolved.
					MutationResult.VelocityVector.valueAt (originalByComputationEdgesPair.Key);

				FlowsByEdges.Add (originalByComputationEdgesPair.Value, Qvalue);
				VelocitiesByEdges.Add (originalByComputationEdgesPair.Value, VelocityValue);

				var startEndNodesFinder = new FindStartEndNodesOfAbstractEdge ();
				originalByComputationEdgesPair.Value.accept (startEndNodesFinder);

				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.EndNode] += Qvalue;
				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.StartNode] -= Qvalue;
								
				var originalEdge = originalByComputationEdgesPair.Value;
				findEdgeAnomalies (
					originalByComputationEdgesPair.Key, 
					Qvalue, 
					VelocityValue,
					originalEdge);

			}

			foreach (var originalByComputationNodesPair in 
			         fluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {
				
				var originalNode = originalByComputationNodesPair.Value;
				
				var nodePressure = PressuresByNodes [originalNode];
				var algebraicSumOfFlows = AlgebraicSumOfFlowsByNodes [originalNode];

				findNodeAnomalies (originalByComputationNodesPair.Key.Identifier, 
					originalNode,
					nodePressure,
					algebraicSumOfFlows);

			}
		}

		class FindStartEndNodesOfAbstractEdge : GasEdgeVisitor
		{
			public GasNodeAbstract StartNode{ get; private set; }

			public GasNodeAbstract EndNode{ get; private set; }

			#region GasEdgeVisitor implementation

			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				gasEdgePhysical.Described.accept (this);
			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				StartNode = gasEdgeTopological.StartNode;
				EndNode = gasEdgeTopological.EndNode;
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				// ignore the gadget since we want connection
				// informations only
				gasEdgeWithGadget.Equipped.accept (this);
			}

			#endregion
		}

		protected virtual TimeSpan? computeElapsedTimeFrom (OneStepMutationResults mutationResults)
		{
			TimeSpan? timeSpan = null;
			if (mutationResults.ComputationStartTimestamp.HasValue &&
			    mutationResults.ComputationEndTimestamp.HasValue) {
				timeSpan = mutationResults.ComputationEndTimestamp.Value.Subtract (
					mutationResults.ComputationStartTimestamp.Value);
			}

			return timeSpan;
		}

		public void forNegativeLoadsCorrectedState (FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			PressuresByNodes = new Dictionary<GasNodeAbstract, double> ();
			AlgebraicSumOfFlowsByNodes = new Dictionary<GasNodeAbstract, double> ();
			FlowsByEdges = new Dictionary<GasEdgeAbstract, double> ();
			VelocitiesByEdges = new Dictionary<GasEdgeAbstract, double> ();
			AnomaliesByNodes = new Dictionary<GasNodeAbstract, StringBuilder> ();
			AnomaliesByEdges = new Dictionary<GasEdgeAbstract, StringBuilder> ();

			ElapsedTime = computeElapsedTimeFrom (fluidDynamicSystemStateNegativeLoadsCorrected.
			                                      FluidDynamicSystemStateMathematicallySolved.MutationResult);

			var dimensionalUnknowns = fluidDynamicSystemStateNegativeLoadsCorrected.FluidDynamicSystemStateMathematicallySolved.
				MutationResult.makeUnknownsDimensional ().WrappedObject;

			var originalNodesBySubstitutedNodes = fluidDynamicSystemStateNegativeLoadsCorrected.NodesSubstitutions.OriginalsBySubstituted ();
			foreach (var aNodePair in fluidDynamicSystemStateNegativeLoadsCorrected.
			         FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

				var originalNode = originalNodesBySubstitutedNodes.ContainsKey (aNodePair.Value) ?
					originalNodesBySubstitutedNodes [aNodePair.Value] : aNodePair.Value;

				PressuresByNodes.Add (originalNode, dimensionalUnknowns.valueAt (aNodePair.Key));
				AlgebraicSumOfFlowsByNodes.Add (originalNode, 0d);
			}

			var originalEdgesBySubstitutedNodes = fluidDynamicSystemStateNegativeLoadsCorrected.
				EdgesSubstitutions.OriginalsBySubstituted ();
			foreach (var edgePair in fluidDynamicSystemStateNegativeLoadsCorrected.
			         FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalEdgesByComputationEdges) {

				var originalEdge = originalEdgesBySubstitutedNodes.ContainsKey (edgePair.Value) ?
					originalEdgesBySubstitutedNodes [edgePair.Value] : edgePair.Value; 

				var Qvalue = fluidDynamicSystemStateNegativeLoadsCorrected.
					FluidDynamicSystemStateMathematicallySolved.MutationResult.
						Qvector.valueAt (edgePair.Key);

				var VelocityValue = fluidDynamicSystemStateNegativeLoadsCorrected.
					FluidDynamicSystemStateMathematicallySolved.MutationResult.
						VelocityVector.valueAt (edgePair.Key);

				FlowsByEdges.Add (originalEdge, Qvalue);
				VelocitiesByEdges.Add (originalEdge, VelocityValue);
				
				var startEndNodesFinder = new FindStartEndNodesOfAbstractEdge ();
				originalEdge.accept (startEndNodesFinder);

				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.EndNode] += Qvalue;
				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.StartNode] -= Qvalue;
				
				findEdgeAnomalies (
					edgePair.Key, 
					Qvalue, 
					VelocityValue,
					originalEdge);
			}

			foreach (var aNodePair in fluidDynamicSystemStateNegativeLoadsCorrected.
			         FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

				var originalNode = originalNodesBySubstitutedNodes.ContainsKey (aNodePair.Value) ?
					originalNodesBySubstitutedNodes [aNodePair.Value] : aNodePair.Value;
				
				var nodePressure = PressuresByNodes [originalNode];
				var algebraicSumOfFlows = AlgebraicSumOfFlowsByNodes [originalNode];

				findNodeAnomalies (aNodePair.Key.Identifier, 
					originalNode,
					nodePressure,
					algebraicSumOfFlows);
			}
		}

		#endregion
	}
}

