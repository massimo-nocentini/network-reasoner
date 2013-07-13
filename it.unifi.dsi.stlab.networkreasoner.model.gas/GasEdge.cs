using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdge
	{
		public GasEdge ()
		{
		}

		public GasNodeAbstract StartNode{ get; set; }

		public GasNodeAbstract EndNode{ get; set; }

		public long Length{ get; set; }

		public double Roughness{ get; set; }

		public double Diameter{ get; set; }

		public double MaxSpeed{ get; set; }
	}
}

