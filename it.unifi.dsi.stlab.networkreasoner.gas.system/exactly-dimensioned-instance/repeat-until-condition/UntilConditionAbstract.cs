using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public abstract class UntilConditionAbstract
	{
		public abstract Boolean canContinue (
			OneStepMutationResults previousOneStepMutationResults,
			OneStepMutationResults currentOneStepMutationResults);

		public abstract string stoppingCauseDescription ();

	}
}

