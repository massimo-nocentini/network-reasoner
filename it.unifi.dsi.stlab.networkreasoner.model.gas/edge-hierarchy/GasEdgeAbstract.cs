using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasEdgeAbstract
	{
		public abstract void accept (GasEdgeVisitor aVisitor);
	}
}

