using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasNodeAbstract
	{
		public abstract void accept (GasNodeVisitor visitor);
	}
}

