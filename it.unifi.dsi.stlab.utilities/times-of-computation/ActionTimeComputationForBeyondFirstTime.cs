using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public class ActionTimeComputationForBeyondFirstTime : ActionTimeComputation
	{
		#region implemented abstract members of it.unifi.dsi.stlab.utilities.times_of_computation.ActionTimeComputation
		internal override void performFromBeyondFirstTime ()
		{
			this.Action.Invoke ();
		}

		internal override void performFromFirstTime ()
		{
			// we do not do nothing since this method is called only
			// the first time of the computation.
		}
		#endregion
	}
}

