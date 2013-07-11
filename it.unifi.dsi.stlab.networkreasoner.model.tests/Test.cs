using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.model.tests
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestCase ()
		{
			NetworkObject userGasVertex = GasFactory.makeGRFVertex();
			Assert.NotNull(userGasVertex);
		}



	}
}

