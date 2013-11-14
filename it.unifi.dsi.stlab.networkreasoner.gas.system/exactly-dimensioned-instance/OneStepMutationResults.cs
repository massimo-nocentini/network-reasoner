using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public  class OneStepMutationResults
	{
		public Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix {
			get;
			set;
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> Unknowns {
			get;
			set;
		}

		public Vector<NodeForNetwonRaphsonSystem> Coefficients {
			get;
			set;
		}

		public Vector<EdgeForNetwonRaphsonSystem> Qvector {
			get;
			set;
		}

		public Matrix<NodeForNetwonRaphsonSystem, 
					NodeForNetwonRaphsonSystem> Jacobian {
			get;
			set;
		}

		public Vector<EdgeForNetwonRaphsonSystem> Fvector {
			get;
			set;
		}

		public int IterationNumber {
			get;
			set;
		}

		public NetwonRaphsonSystem ComputedBy {
			get;
			set;
		}

		public NodeForNetwonRaphsonSystem findNodeByIdentifier (
			String identifier)
		{
			return this.ComputedBy.Nodes.Find (
				aNode => aNode.Identifier.Equals (identifier));
		}

		public EdgeForNetwonRaphsonSystem findEdgeByIdentifier (
			String identifier)
		{
			return this.ComputedBy.Edges.Find (
				anEdge => anEdge.Identifier.Equals (identifier));
		}

		public EdgeForNetwonRaphsonSystem findEdgeByStartEndNodes (
			NodeForNetwonRaphsonSystem startNode, 
			NodeForNetwonRaphsonSystem endNode)
		{
			return this.ComputedBy.Edges.Find (
				anEdge => anEdge.StartNode.Equals (startNode) && 
				anEdge.EndNode.Equals (endNode)
			);
		}


	}
}

