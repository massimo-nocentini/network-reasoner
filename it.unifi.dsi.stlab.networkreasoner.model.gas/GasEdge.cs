using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdge
	{
		public GasEdge ()
		{
		}

		public GasNode StartNode{ get; set; }

		public GasNode EndNode{ get; set; }

		public long Length{ get; set; }

		public double Roughness{ get; set; }

		public double Diameter{ get; set; }

		public double MaxSpeed{ get; set; }
	}
}

