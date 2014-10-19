using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	abstract class MutateComputationDriver
	{
		public abstract bool canDoOneMoreStep ();

		public abstract void doOneMoreStep (Action step);
	}

	class MutateComputationDriverDoOneMoreMutation : MutateComputationDriver
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver

		public override bool canDoOneMoreStep ()
		{
			return true;
		}

		public override void doOneMoreStep (Action step)
		{
			step.Invoke ();
		}

		#endregion
	}

	class MutateComputationDriverStopMutate : MutateComputationDriver
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver

		public override bool canDoOneMoreStep ()
		{
			return false;
		}

		public override void doOneMoreStep (Action step)
		{
			// nothing to do with step since we've to interrupt the computation
		}

		#endregion
	}
}

