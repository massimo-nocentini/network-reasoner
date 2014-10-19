using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public class NodeRolePassive:NodeRoleLoader
	{
		public NodeRolePassive ()
		{
			this.Load = 0d;
		}

		public override void putYourCoefficientIntoFor (
			NodeForNetwonRaphsonSystem aNode, 
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			aVector.atPut (aNode, 0d);
		}
	}
}

