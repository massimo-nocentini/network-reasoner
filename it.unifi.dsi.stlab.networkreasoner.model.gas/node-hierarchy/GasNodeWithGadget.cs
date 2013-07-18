using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeWithGadget : GasNodeAbstract
	{
		public GasNodeAbstract Equipped{ get; set; }

		public GasNodeGadget Gadget{ get; set; }

		public override void accept (GasNodeVisitor visitor)
		{
			visitor.forNodeWithGadget (this);
		}
	

	}
}

