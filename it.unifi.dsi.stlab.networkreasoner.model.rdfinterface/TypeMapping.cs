using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface
{
	public class TypeMapping
	{
		public static Dictionary<String, Type> Mapping{ get; private set; }

		static TypeMapping ()
		{
			TypeMapping.Mapping = new Dictionary<string, Type> ();
			TypeMapping.Mapping.Add ("gas-node", typeof(GasNode));
		}
	}
}

