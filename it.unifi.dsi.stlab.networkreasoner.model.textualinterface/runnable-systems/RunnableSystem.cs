using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public interface RunnableSystem
	{
		void compute (
			String systemName,
			Dictionary<string, GasNodeAbstract> nodes, 
			Dictionary<string, GasEdgeAbstract> edges, 
			AmbientParameters ambientParameters);
	}
}

