using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasEdgeTopological : GasEdgeAbstract
	{
		public GasNodeAbstract StartNode{ get; set; }

		public GasNodeAbstract EndNode{ get; set; }
	}
}

