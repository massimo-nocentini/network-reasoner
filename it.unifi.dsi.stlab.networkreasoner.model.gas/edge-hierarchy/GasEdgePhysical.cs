using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgePhysical : GasEdgeAbstract
	{
		public GasEdgeAbstract Described{ get; set; }

		public double Length{ get; set; }

		public double Roughness{ get; set; }

		public double Diameter{ get; set; }

		public double? MaxSpeed{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasEdgeAbstract
		public override void accept (GasEdgeVisitor aVisitor)
		{
			aVisitor.forPhysicalEdge (this);
		}
		#endregion

	}
}

