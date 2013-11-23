using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class RunnableSystemWithDecoration : RunnableSystem
	{
		public RunnableSystem DecoredRunnableSystem { get; set; }

		#region RunnableSystem implementation
		public virtual FluidDynamicSystemStateAbstract compute (
			string systemName, 
			Dictionary<string, GasNodeAbstract> nodes, 
			Dictionary<string, GasEdgeAbstract> edges, 
			AmbientParameters ambientParameters)
		{
			return DecoredRunnableSystem.compute(systemName, nodes, edges, ambientParameters);
		}
		#endregion

	}
}

