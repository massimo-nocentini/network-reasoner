using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeGadgetSupply : GasNodeGadget
	{

		public double SetupPressure { get; set; }

		public double MaxQ { get; set; }

		public double MinQ { get; set; }

		public override void accept (GasNodeGadgetVisitor visitor)
		{
			visitor.forSupplyGadget (this);
		}


	}
}

