using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using System.Linq;
using it.unifi.dsi.stlab.extensionmethods;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionInitialization : FluidDynamicSystemStateTransition
	{
		public UnknownInitialization UnknownInitialization{ get; set; }
		
		public GasNetwork Network { get; set; }
		
		public Func<double, double> FromDimensionalToAdimensionalTranslator{ get; set; }
	
		#region FluidDynamicSystemStateOperation implementation
		public virtual FluidDynamicSystemStateAbstract forBareSystemState (
			FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			var unsolvedState = new FluidDynamicSystemStateUnsolved ();

			// TODO: maybe this dictionary can be useful in the rest of computation?
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode;
		
			initializeNodes (out newtonRaphsonNodesByOriginalNode, unsolvedState);

			initializeEdges (newtonRaphsonNodesByOriginalNode, unsolvedState);

			unsolvedState.OriginalNetwork = Network;
			unsolvedState.InitializedBy = this;

			return unsolvedState;
		}

		protected virtual void initializeNodes (
			out Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode,
			FluidDynamicSystemStateUnsolved unsolvedState)
		{
			var originalNodesMapping = new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			unsolvedState.InitialUnknownVector = new DimensionalObjectWrapperWithAdimensionalValues<
				Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = new Vector<NodeForNetwonRaphsonSystem> ()
			};

			var initialUnknownGuessVector = this.makeInitialGuessForUnknowns ();

			unsolvedState.OriginalNodesByComputationNodes = 
				new Dictionary<NodeForNetwonRaphsonSystem, GasNodeAbstract> ();

			Network.doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aNode => {

				var newtonRaphsonNode = new NodeForNetwonRaphsonSystem ();
				newtonRaphsonNode.initializeWith (aNode);

				originalNodesMapping.Add (aNode, newtonRaphsonNode);

				unsolvedState.InitialUnknownVector.WrappedObject.atPut (newtonRaphsonNode, 
				                                        initialUnknownGuessVector [aNode]);

				unsolvedState.OriginalNodesByComputationNodes.Add (newtonRaphsonNode, aNode);
			}
			)
			);

			newtonRaphsonNodesByOriginalNode = originalNodesMapping;

			var computationalNodes = originalNodesMapping.Values.ToList ();
			unsolvedState.Nodes = computationalNodes;
			unsolvedState.NodesEnumeration = computationalNodes.enumerate ();
		}

		protected virtual Dictionary<GasEdgeAbstract, double> makeInitialGuessForFvector ()
		{
			var initialFvector = new Dictionary<GasEdgeAbstract, double> ();

			Network.doOnEdges (new NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => initialFvector.Add (anEdge, .015))
			);

			return initialFvector;
		}

		protected virtual Dictionary<GasNodeAbstract, double> makeInitialGuessForUnknowns ()
		{
			var initialUnknowns = new  Dictionary<GasNodeAbstract, double> ();
			var rand = new Random (DateTime.Now.Millisecond);

			Network.doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => {

				var value = UnknownInitialization.initialValueFor (aVertex, rand);

				initialUnknowns.Add (aVertex, value.makeAdimensional (
					FromDimensionalToAdimensionalTranslator).WrappedObject
				);
			}
			)
			);

			return initialUnknowns;
		}

		protected virtual void initializeEdges (
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode,
			FluidDynamicSystemStateUnsolved unsolvedState)
		{
			unsolvedState.Edges = new List<EdgeForNetwonRaphsonSystem> ();
			unsolvedState.InitialFvector = new Vector<EdgeForNetwonRaphsonSystem> ();
			unsolvedState.OriginalEdgesByComputationEdges = 
				new Dictionary<EdgeForNetwonRaphsonSystem, GasEdgeAbstract> ();

			var initialFvalueGuessVector = this.makeInitialGuessForFvector ();

			Network.doOnEdges (new NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (anEdge => {
				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder {
					CustomNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode
				};
				var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);
				unsolvedState.Edges.Add (edgeForNetwonRaphsonSystem);
				unsolvedState.InitialFvector.atPut (edgeForNetwonRaphsonSystem, initialFvalueGuessVector [anEdge]);
				unsolvedState.OriginalEdgesByComputationEdges.Add (edgeForNetwonRaphsonSystem, anEdge);
			}
			)
			);

			unsolvedState.EdgesEnumeration = unsolvedState.Edges.enumerate ();
		}

		public FluidDynamicSystemStateAbstract forUnsolvedSystemState (
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			// simply return the given state since if it is unsolved someone initialized it already.
			return fluidDynamicSystemStateUnsolved;
		}

		public FluidDynamicSystemStateAbstract forMathematicallySolvedState (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			// simply return the given state since if it is mathematically solved someone initialized it already.
			return fluidDynamicSystemStateMathematicallySolved;
		}

		public FluidDynamicSystemStateAbstract forNegativeLoadsCorrectedState (
			FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			// simply return the given state since if it is already checked for negative pressures someone initialized it already.
			return fluidDynamicSystemStateNegativeLoadsCorrected;
		}

		public virtual FluidDynamicSystemStateTransition clone ()
		{
			var clone = new FluidDynamicSystemStateTransitionInitialization ();
			clone.FromDimensionalToAdimensionalTranslator = FromDimensionalToAdimensionalTranslator;
			clone.Network = Network;
			clone.UnknownInitialization = UnknownInitialization;
			return clone;
		}
		#endregion
	}
}

