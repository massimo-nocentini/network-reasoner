using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeGadgetLoad : GasNodeGadget
	{
		public double Load { get; set; }

		public override void accept (GasNodeGadgetVisitor visitor)
		{
			visitor.forLoadGadget (this);
		}


	}
}

