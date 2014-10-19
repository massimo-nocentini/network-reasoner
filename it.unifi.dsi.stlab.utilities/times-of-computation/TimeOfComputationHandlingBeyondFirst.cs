using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public class TimeOfComputationHandlingBeyondFirst : TimeOfComputationHandling
	{
		#region implemented abstract members of it.unifi.dsi.stlab.utilities.times_of_computation.TimeOfComputationHandling

		public override TimeOfComputationHandling advance ()
		{
			return this;
		}

		public override void perform (ActionTimeComputation aTimeComputationAction)
		{
			aTimeComputationAction.performFromBeyondFirstTime ();
		}

		#endregion


	}
}

