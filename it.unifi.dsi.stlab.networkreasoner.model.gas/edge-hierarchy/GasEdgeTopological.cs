using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgeTopological : GasEdgeAbstract
	{
		public string Identifier{ get; set; }

		public GasNodeAbstract StartNode{ get; set; }

		public GasNodeAbstract EndNode{ get; set; }
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasEdgeAbstract
		public override void accept (GasEdgeVisitor aVisitor)
		{
			aVisitor.forTopologicalEdge (this);
		}
		#endregion

	}
}

