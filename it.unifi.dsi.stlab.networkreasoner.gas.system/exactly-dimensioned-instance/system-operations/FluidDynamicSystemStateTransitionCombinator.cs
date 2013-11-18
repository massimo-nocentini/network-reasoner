using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionCombinator : FluidDynamicSystemStateTransition
	{
		public FluidDynamicSystemStateAbstract applySequenceOnBareState (
			List<FluidDynamicSystemStateTransition> transitionSequence)
		{
			FluidDynamicSystemStateAbstract state = new FluidDynamicSystemStateBare ();

			transitionSequence.ForEach (aTransition => state = state.doStateTransition (aTransition));

			return state;
		}

		#region FluidDynamicSystemStateTransition implementation
		public FluidDynamicSystemStateAbstract forBareSystemState (FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			throw new System.NotImplementedException ();
		}

		public FluidDynamicSystemStateAbstract forUnsolvedSystemState (FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			throw new System.NotImplementedException ();
		}

		public FluidDynamicSystemStateAbstract forMathematicallySolvedState (FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			throw new System.NotImplementedException ();
		}

		public FluidDynamicSystemStateAbstract forNegativeLoadsCorrectedState (FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			throw new System.NotImplementedException ();
		}

		public FluidDynamicSystemStateTransition clone ()
		{
			throw new System.NotImplementedException ();
		}
		#endregion
	}
}

