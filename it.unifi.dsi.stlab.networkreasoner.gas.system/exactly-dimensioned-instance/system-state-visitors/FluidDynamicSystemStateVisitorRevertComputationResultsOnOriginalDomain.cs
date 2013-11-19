using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.extensionmethods;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain
		: FluidDynamicSystemStateVisitor
	{
		public Dictionary<GasEdgeAbstract, double> FlowsByEdges {
			get;
			set;
		}

		public Dictionary<GasNodeAbstract, double> PressuresByNodes {
			get;
			set;
		}

		#region FluidDynamicSystemStateVisitor implementation
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

		public void forMathematicallySolvedState (FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			var dimensionalUnknowns = fluidDynamicSystemStateMathematicallySolved.
				MutationResult.makeUnknownsDimensional ().WrappedObject;

			foreach (var originalByComputationNodesPair in 
			         fluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

				PressuresByNodes.Add (originalByComputationNodesPair.Value, 
				                      dimensionalUnknowns.valueAt (originalByComputationNodesPair.Key));
			}

			foreach (var originalByComputationEdgesPair in
			        fluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalEdgesByComputationEdges) {
			
				FlowsByEdges.Add (originalByComputationEdgesPair.Value,
				                 fluidDynamicSystemStateMathematicallySolved.
				                  MutationResult.Qvector.valueAt (
					originalByComputationEdgesPair.Key)
				);
			}
		}

		public void forNegativeLoadsCorrectedState (FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			var dimensionalUnknowns = fluidDynamicSystemStateNegativeLoadsCorrected.FluidDynamicSystemStateMathematicallySolved.
				MutationResult.makeUnknownsDimensional ().WrappedObject;

			var originalNodesBySubstitutedNodes = fluidDynamicSystemStateNegativeLoadsCorrected.NodesSubstitutions.OriginalsBySubstituted ();
			foreach (var aNodePair in fluidDynamicSystemStateNegativeLoadsCorrected.
			         FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

				var originalNode = originalNodesBySubstitutedNodes.ContainsKey (aNodePair.Value) ?
					originalNodesBySubstitutedNodes [aNodePair.Value] : aNodePair.Value;

				PressuresByNodes.Add (originalNode, dimensionalUnknowns.valueAt (aNodePair.Key));
			}

			var originalEdgesBySubstitutedNodes = fluidDynamicSystemStateNegativeLoadsCorrected.
				EdgesSubstitutions.OriginalsBySubstituted ();
			foreach (var edgePair in fluidDynamicSystemStateNegativeLoadsCorrected.
			         FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalEdgesByComputationEdges) {

				var originalEdge = originalEdgesBySubstitutedNodes.ContainsKey (edgePair.Value) ?
					originalEdgesBySubstitutedNodes [edgePair.Value] : edgePair.Value; 

				FlowsByEdges.Add (originalEdge,
				                fluidDynamicSystemStateNegativeLoadsCorrected.
				                FluidDynamicSystemStateMathematicallySolved.
				                    MutationResult.Qvector.valueAt (edgePair.Key)
				);
			}
		}
		#endregion


	}
}

