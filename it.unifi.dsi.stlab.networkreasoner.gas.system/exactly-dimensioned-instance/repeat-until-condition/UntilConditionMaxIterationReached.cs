using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class UntilConditionMaxIterationReached : 
		UntilConditionAbstract
	{
		public int MaxNumberOfInterations {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.UntilConditionAbstract
		public override bool canContinue (
			OneStepMutationResults previousOneStepMutationResults,
			OneStepMutationResults currentOneStepMutationResults)
		{
			return currentOneStepMutationResults.IterationNumber <
				this.MaxNumberOfInterations;
		}
		#endregion

	}
}

