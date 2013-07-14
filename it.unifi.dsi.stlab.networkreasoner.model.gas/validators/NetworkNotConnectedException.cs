using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetworkNotConnectedException : Exception
	{
		public NetworkNotConnectedException (String message):base(message)
		{
		}
	}
}

