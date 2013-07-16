using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgeGadgetSwitchOff : GasEdgeGadget
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasEdgeGadget
		public override void accept (GasEdgeGadgetVisitor aVisitor)
		{
			aVisitor.forSwitchOffGadget (this);
		}
		#endregion

	}
}

