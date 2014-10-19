using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateUnsolved : FluidDynamicSystemStateAbstract
	{
		public GasNetwork OriginalNetwork { get; set; }

		public List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		public Dictionary<NodeForNetwonRaphsonSystem, int> NodesEnumeration { get; set; }

		public Dictionary<EdgeForNetwonRaphsonSystem, int> EdgesEnumeration { get; set; }

		public Dictionary<NodeForNetwonRaphsonSystem, GasNodeAbstract> OriginalNodesByComputationNodes { get; set; }

		public Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ComputationNodesByOriginalNodes { get; set; }

		public Dictionary<EdgeForNetwonRaphsonSystem, GasEdgeAbstract> OriginalEdgesByComputationEdges { get; set; }

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> InitialUnknownVector { get; set; }

		public Vector<EdgeForNetwonRaphsonSystem> InitialFvector{ get; set; }

		public FluidDynamicSystemStateTransitionInitialization InitializedBy{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.FluidDynamicSystemStateAbstract

		public override FluidDynamicSystemStateAbstract doStateTransition (FluidDynamicSystemStateTransition aVisitor)
		{
			return aVisitor.forUnsolvedSystemState (this);
		}

		public override void accept (FluidDynamicSystemStateVisitor aVisitor)
		{
			aVisitor.forUnsolvedSystemState (this);
		}

		#endregion

		public NodeForNetwonRaphsonSystem findNodeByIdentifier (
			String identifier)
		{
			return Nodes.Find (aNode => aNode.Identifier.Equals (identifier));
		}

		public EdgeForNetwonRaphsonSystem findEdgeByIdentifier (
			String identifier)
		{
			return Edges.Find (anEdge => anEdge.Identifier.Equals (identifier));
		}

		public EdgeForNetwonRaphsonSystem findEdgeByStartEndNodes (
			NodeForNetwonRaphsonSystem startNode, 
			NodeForNetwonRaphsonSystem endNode)
		{
			return Edges.Find (anEdge => anEdge.StartNode.Equals (startNode) &&
			anEdge.EndNode.Equals (endNode)
			);
		}

	}
}

