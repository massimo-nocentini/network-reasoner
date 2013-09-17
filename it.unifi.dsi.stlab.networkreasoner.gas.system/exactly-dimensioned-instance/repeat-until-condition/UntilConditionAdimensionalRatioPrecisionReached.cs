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
			var ratioVector = currentOneStepMutationResults.Unknowns.ratio (
				previousOneStepMutationResults.Unknowns);

			return ratioVector.TrueForAll (element => {

				var ratio = ratioVector.valueAt (element);

				return ratio > (this.Precision + 1) || ratio < (-this.Precision + 1);}
			);
		}
		#endregion

	}
}

