using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public abstract class ActionTimeComputation
	{
		public Action Action{ get; set; }

		internal abstract void performFromBeyondFirstTime ();

		internal abstract void performFromFirstTime ();

	}
}

