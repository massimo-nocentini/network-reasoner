using System;

namespace it.unifi.dsi.stlab.utilities.times_of_computation
{
	public class TimeOfComputationHandlingFirstOne : TimeOfComputationHandling
	{
		public override TimeOfComputationHandling advance ()
		{
			return new TimeOfComputationHandlingBeyondFirst ();
		}

		public override void perform (ActionTimeComputation aTimeComputationAction)
		{
			aTimeComputationAction.performFromFirstTime ();
		}
		


	}
}

