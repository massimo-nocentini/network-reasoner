using System;
using NUnit.Framework;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class Test
	{
		private MeasureUnitFactory MeasureUnitFactory { get; set; }

		public Test ()
		{
			this.MeasureUnitFactory = new MeasureUnitFactory ();
		}

		[Test()]
		public void TestCase ()
		{
			GasVertexBuilder gasVertexBuilder = GasFactory.vertexBuilder ();

			ObservationMethod observationMethod = null;
			Vertex v = gasVertexBuilder.
				isGRF ().
					hasPressure (this.MeasureUnitFactory.bar ().
					             netwon (45.7).
					             square_centimeters (35.2).
					             make (),
				   	         observationMethod.imposed ()).
				build ();

			Assert.NotNull (v);
		}
	}
}

