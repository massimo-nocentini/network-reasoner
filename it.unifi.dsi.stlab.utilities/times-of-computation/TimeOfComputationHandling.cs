using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public abstract class TimeOfComputationHandling
	{
		public abstract TimeOfComputationHandling advance ();

		public abstract void perform (ActionTimeComputation aTimeComputationAction);	

	}
}

