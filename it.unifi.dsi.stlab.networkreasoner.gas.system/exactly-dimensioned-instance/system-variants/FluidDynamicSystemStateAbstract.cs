using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public abstract class FluidDynamicSystemStateAbstract
	{
		public abstract FluidDynamicSystemStateAbstract doStateTransition (FluidDynamicSystemStateTransition aVisitor);

		public abstract void accept (FluidDynamicSystemStateVisitor aVisitor);
	}
}

