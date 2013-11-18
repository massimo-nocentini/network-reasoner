using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public abstract class FluidDynamicSystemStateAbstract
	{
		public abstract FluidDynamicSystemStateAbstract doStateTransition (FluidDynamicSystemStateTransition aVisitor);
	}
}

