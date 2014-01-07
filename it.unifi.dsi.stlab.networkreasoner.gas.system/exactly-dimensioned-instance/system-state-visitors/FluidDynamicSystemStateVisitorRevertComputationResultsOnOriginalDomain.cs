using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.extension_methods;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

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

		public void forMathematicallySolvedState (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			PressuresByNodes = new Dictionary<GasNodeAbstract, double> ();
			AlgebraicSumOfFlowsByNodes = new Dictionary<GasNodeAbstract, double> ();
			FlowsByEdges = new Dictionary<GasEdgeAbstract, double> ();
			VelocitiesByEdges = new Dictionary<GasEdgeAbstract, double> ();

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
			        fluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalEdgesByComputationEdges) {

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

				var velocityValue = fluidDynamicSystemStateNegativeLoadsCorrected.
					FluidDynamicSystemStateMathematicallySolved.MutationResult.
						VelocityVector.valueAt (edgePair.Key);

				FlowsByEdges.Add (originalEdge, Qvalue);
				VelocitiesByEdges.Add (originalEdge, velocityValue);
				
				var startEndNodesFinder = new FindStartEndNodesOfAbstractEdge ();
				originalEdge.accept (startEndNodesFinder);

				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.EndNode] += Qvalue;
				AlgebraicSumOfFlowsByNodes [startEndNodesFinder.StartNode] -= Qvalue;
			}
		}
		#endregion
	}
}

