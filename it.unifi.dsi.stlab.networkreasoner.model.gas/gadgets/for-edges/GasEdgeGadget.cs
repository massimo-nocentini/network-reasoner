using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasEdgeGadget
	{
		public abstract void accept (GasEdgeGadgetVisitor aVisitor);
	}
}

