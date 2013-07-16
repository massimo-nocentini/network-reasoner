using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class ReachabilityValidator : ValidatorAbstract
	{


		interface NodeState
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

		interface NodeRole
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

		class NodeForReachabilityValidator : GasNodeVisitor, GasNodeGadgetVisitor
		{
			String Identifier{ get; set; }

			List<NodeForReachabilityValidator> Neighborhood{ get; set; }

			NodeState State{ get; set; }

			NodeRole Role{ get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (
				GasNodeTopological gasNodeTopological)
			{
				this.Identifier = gasNodeTopological.Identifier;
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}
			#endregion			

			#region GasNodeGadgetVisitor implementation
			public void ForLoadGadget (GasNodeGadgetLoad aLoadGadget)
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

		void setupValidatorFor (
			GasNetwork gasNetwork, 
			out List<NodeForReachabilityValidator> nodes)
		{
			nodes = new List<NodeForReachabilityValidator> ();

			var buildingNodes = new Dictionary<String, NodeForReachabilityValidator> ();

			gasNetwork.doOnNodes (new GasNetwork.NodeHandlerWithDelegateOnKeyedNode (
			(aKey, aNode) => {

				var newNode = new NodeForReachabilityValidator {
					State = new NodeStateNotVisited ()
				};

				aNode.accept (newNode);
				buildingNodes.Add (aKey, newNode);
			}
			)
			);
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

		internal virtual void HandleNotVisitedNode (
			NodeForReachabilityValidator aNotVisitedNode)
		{
			throw new NetworkNotConnectedException (string.Format (
					"The node ``{0}'' isn't connected to the others.", 
						aNotVisitedNode.Identifier)
			);
		}
	}
}

