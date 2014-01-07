using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class ParserResultReceiver
	{
		public abstract void receiveResults (Dictionary<string, object> objectsByUri);
	}


}

