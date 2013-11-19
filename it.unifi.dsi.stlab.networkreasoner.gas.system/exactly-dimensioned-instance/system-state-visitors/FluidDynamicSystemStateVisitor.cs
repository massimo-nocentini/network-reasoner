using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public interface FluidDynamicSystemStateVisitor
	{
		void forBareSystemState (FluidDynamicSystemStateBare fluidDynamicSystemStateBare);

		void forUnsolvedSystemState (FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved);

		void forMathematicallySolvedState (FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved);

		void forNegativeLoadsCorrectedState (FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected);
	}
}

