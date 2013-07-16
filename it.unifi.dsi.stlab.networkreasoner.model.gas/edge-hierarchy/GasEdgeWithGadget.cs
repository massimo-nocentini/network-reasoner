using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgeWithGadget : GasEdgeAbstract
	{
		public GasEdgeAbstract Equipped{ get; set; }

		public GasEdgeGadget Gadget{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasEdgeAbstract
		public override void accept (GasEdgeVisitor aVisitor)
		{
			aVisitor.forEdgeWithGadget (this);
		}
		#endregion

	}
}

