using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class UntilConditionAdimensionalRatioPrecisionReached
		: UntilConditionAbstract
	{
		public Double Precision{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.UntilConditionAbstract
		public override bool canContinue (
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			if (previousOneStepMutationResults == null) {
				return true;
			}

			var ratioVector = currentOneStepMutationResults.Unknowns.WrappedObject.ratio (
				previousOneStepMutationResults.Unknowns.WrappedObject);

			return ratioVector.atLeastOneSatisfy (element => {

				var ratio = ratioVector.valueAt (element);

				return ratio > (this.Precision + 1) || ratio < (-this.Precision + 1);}
			);
		}

		public override string stoppingCauseDescription ()
		{
			return string.Format ("the precision {0} against ratios has been reached", this.Precision);
		}
		#endregion


	}
}

