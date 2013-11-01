using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public class ActionTimeComputationOnFirstTime : ActionTimeComputation
	{
		#region implemented abstract members of it.unifi.dsi.stlab.utilities.times_of_computation.ActionTimeComputation
		internal override void performFromBeyondFirstTime ()
		{
			// we do not do nothing since this method is called only
			// after the first time of the computation.
		}

		internal override void performFromFirstTime ()
		{
			this.Action.Invoke ();
		}
		#endregion
	}
}

