using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class ReachabilityValidator : ValidatorAbstract
	{


		protected interface NodeState
		{
			void visitFor (NodeForReachabilityValidator aNode);

			void assertVisitedSignalingIfFalseFor (
				NodeForReachabilityValidator aNode, 
				Action<NodeForReachabilityValidator> aBlockIfAssertFails);

		}

		class NodeStateVisited : NodeState
		{
			#region NodeState implementation
			public void visitFor (NodeForReachabilityValidator aNode)
			{
				// since the vertex is already visited we haven't to 
				// continue the visiting of its neighborhood.
			}

			public void assertVisitedSignalingIfFalseFor (
				NodeForReachabilityValidator aNode, 
				Action<NodeForReachabilityValidator> aBlockIfAssertFails)
			{
				// nothing to raise since the node is visited.
			}
			#endregion
		}

		class NodeStateNotVisited : NodeState
		{
			#region NodeState implementation
			public void visitFor (NodeForReachabilityValidator aNode)
			{
				aNode.State = new NodeStateVisited ();
				aNode.Neighborhood.ForEach (neighbor => neighbor.visit ());
			}

			public void assertVisitedSignalingIfFalseFor (
				NodeForReachabilityValidator aNode, 
				Action<NodeForReachabilityValidator> aBlockIfAssertFails)
			{
				// since the node isn't visited we have to signal the anomaly
				// using the given block.
				aBlockIfAssertFails.Invoke (aNode);
			}
			#endregion
		}

		protected interface NodeRole
		{
			void startVisitFor (NodeForReachabilityValidator aNode);
		}

		class NodeRoleSupplier : NodeRole
		{
			#region NodeRole implementation
			public void startVisitFor (NodeForReachabilityValidator aNode)
			{
				aNode.visit ();
			}
			#endregion
		}

		class NodeRoleLoader : NodeRole
		{
			#region NodeRole implementation
			public void startVisitFor (NodeForReachabilityValidator aNode)
			{
				// since aNode is a node with a load gadget, we can say
				// that it is a loader, hence it cannot start a reachability
				// visit since it cannot supply pressure.
			}
			#endregion
		}

		protected class NodeForReachabilityValidator : 
		GasNodeVisitor, 
			GasNodeGadgetVisitor
		{
			public	String Identifier{ get; set; }

			public	List<NodeForReachabilityValidator> Neighborhood{ get; set; }

			public	NodeState State{ get; set; }

			public	NodeRole Role{ get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (
				GasNodeTopological gasNodeTopological)
			{
				this.Identifier = gasNodeTopological.Identifier;
			}

			public void forNodeWithGadget (
				GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}

			public void forNodeAntecedentInPressureReduction (
				GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
			{
				gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
			}
			#endregion			

			#region GasNodeGadgetVisitor implementation
			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				this.Role = new NodeRoleLoader ();
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				this.Role = new NodeRoleSupplier ();
			}
			#endregion

			public void visit ()
			{
				this.State.visitFor (this);
			}

			public void startVisit ()
			{
				this.Role.startVisitFor (this);
			}

			public void assertVisitedSignalingIfFalse (
				Action<NodeForReachabilityValidator> aBlockIfAssertFails)
			{
				this.State.assertVisitedSignalingIfFalseFor (this, aBlockIfAssertFails);
			}

		}

		protected class EdgeForReachabilityValidator : GasEdgeVisitor
		{
			Dictionary<GasNodeAbstract, NodeForReachabilityValidator> newNodesByKey{ get; set; }

			public EdgeForReachabilityValidator (
				Dictionary<GasNodeAbstract, NodeForReachabilityValidator> newNodesByKey)
			{
				this.newNodesByKey = newNodesByKey;
			}

			#region GasEdgeVisitor implementation
			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				// we don't care about the physical properties.
				gasEdgePhysical.Described.accept (this);
			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				var startVertex = newNodesByKey [gasEdgeTopological.StartNode];
				var endVertex = newNodesByKey [gasEdgeTopological.EndNode];

				startVertex.Neighborhood.Add (endVertex);
				endVertex.Neighborhood.Add (startVertex);
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				gasEdgeWithGadget.Gadget.accept (
					new GasEdgeGadgetVisitorStopRecursionOnSwitchOff (
					this, gasEdgeWithGadget.Equipped)
				);
			}
			#endregion

			class GasEdgeGadgetVisitorStopRecursionOnSwitchOff : GasEdgeGadgetVisitor
			{
				EdgeForReachabilityValidator edgeForReachabilityValidator{ get; set; }

				GasEdgeAbstract equipped{ get; set; }

				public GasEdgeGadgetVisitorStopRecursionOnSwitchOff (
					EdgeForReachabilityValidator edgeForReachabilityValidator, 
					GasEdgeAbstract equipped)
				{
					this.edgeForReachabilityValidator = edgeForReachabilityValidator;
					this.equipped = equipped;
				}


				#region GasEdgeGadgetVisitor implementation
				public void forSwitchOffGadget (
					GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
				{
					// since this gadget turns off the edge we do not proceed
					// the recursive application of the visitor, so we
					// do not reach the very bottom case of TopologicalEdge
					// where the neighborhoods are updated.
				}


				public void forPressureRegulatorGadget (GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator)
				{
					this.equipped.accept (this.edgeForReachabilityValidator);
				}
				#endregion

				// the following is the code for an edge with a switch on gadget
				// (which it doesn't exists up to now).
//				#region GasEdgeGadgetVisitor implementation
//				public void forSwitchOnGadget (
//					GasEdgeGadgetSwitchOn gasEdgeGadgetSwitchOn)
//				{
//					this.equipped.accept (this.edgeForReachabilityValidator);
//				}
//				#endregion
			}


		}

		void setupValidatorFor (
			GasNetwork gasNetwork, 
			out List<NodeForReachabilityValidator> nodes)
		{
			var newNodesByKey = new Dictionary<GasNodeAbstract, NodeForReachabilityValidator> ();

			gasNetwork.doOnNodes (new NodeHandlerWithDelegateOnKeyedNode<GasNodeAbstract> (
			(aKey, aNode) => {

				var newNode = new NodeForReachabilityValidator {
					State = new NodeStateNotVisited (),
					Role = new NodeRoleLoader(),
					Neighborhood = new List<NodeForReachabilityValidator>()
				};

				aNode.accept (newNode);
				newNodesByKey.Add (aNode, newNode);
			}
			)
			);

			EdgeForReachabilityValidator edgeVisitor = 
				new EdgeForReachabilityValidator (newNodesByKey);

			gasNetwork.doOnEdges (new NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => anEdge.accept (edgeVisitor))
			);

			nodes = new List<NodeForReachabilityValidator> (newNodesByKey.Values);

		}

		public void validate (GasNetwork gasNetwork)
		{
			List<NodeForReachabilityValidator> nodes;
			this.setupValidatorFor (gasNetwork, out nodes);

			foreach (var node in nodes) {
				node.startVisit ();
			}

			foreach (var aNode in nodes) {
				aNode.assertVisitedSignalingIfFalse (HandleNotVisitedNode);
			}
		}

		protected virtual void HandleNotVisitedNode (
			NodeForReachabilityValidator aNotVisitedNode)
		{
			throw new NetworkNotConnectedException (string.Format (
					"The node ``{0}'' isn't connected to a vertex with a supply gadget.", 
						aNotVisitedNode.Identifier)
			);
		}

		public class NetworkNotConnectedException : Exception
		{
			public NetworkNotConnectedException (String message):base(message)
			{
			}
		}
	}
}

