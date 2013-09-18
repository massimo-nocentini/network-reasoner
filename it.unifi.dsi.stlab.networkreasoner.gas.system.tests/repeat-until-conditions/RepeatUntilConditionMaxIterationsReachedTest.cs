using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class RepeatUntilConditionMaxIterationsReachedTest
	{
		[Test()]
		public void should_repeat_for_fifteen_times ()
		{
			int maxNumberOfIteration = 15;

			UntilConditionAbstract condition = 
			new UntilConditionMaxIterationReached{
				MaxNumberOfInterations = maxNumberOfIteration
			};

			Assert.IsTrue (condition.canContinue (
				null, new OneStepMutationResults{ IterationNumber = 1})
			);
			Assert.IsTrue (condition.canContinue (
				null, new OneStepMutationResults{ IterationNumber = maxNumberOfIteration -1})
			);
			Assert.IsFalse (condition.canContinue (
				null, new OneStepMutationResults{ IterationNumber = maxNumberOfIteration})
			);
			Assert.IsFalse (condition.canContinue (
				null, new OneStepMutationResults{ IterationNumber = maxNumberOfIteration + 39})
			);
		}
	}
}

