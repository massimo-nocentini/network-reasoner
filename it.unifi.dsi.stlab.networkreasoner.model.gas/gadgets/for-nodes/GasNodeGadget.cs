using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasNodeGadget
	{
		public abstract void accept (GasNodeGadgetVisitor visitor);
	}
}

