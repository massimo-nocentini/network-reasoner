using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public interface RunnableSystem
	{
		FluidDynamicSystemStateAbstract compute (
			String systemName,
			Dictionary<string, GasNodeAbstract> nodes, 
			Dictionary<string, GasEdgeAbstract> edges, 
			AmbientParameters ambientParameters);
	}
}

