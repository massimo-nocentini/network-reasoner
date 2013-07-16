using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgePhysical : GasEdgeAbstract
	{
		public GasEdgeAbstract Equipped{ get; set; }

		public long Length{ get; set; }

		public double Roughness{ get; set; }

		public double Diameter{ get; set; }

		public double MaxSpeed{ get; set; }
	}
}

