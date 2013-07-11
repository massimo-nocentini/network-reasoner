using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasFactory
	{
		public GasFactory ()
		{
		}

		public static GasVertexBuilder vertexBuilder ()
		{
			return new GasVertexBuilder();
		}


	}
}

