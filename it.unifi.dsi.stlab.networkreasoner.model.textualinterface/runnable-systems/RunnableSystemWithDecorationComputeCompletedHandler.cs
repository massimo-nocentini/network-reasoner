using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class RunnableSystemWithDecorationComputeCompletedHandler
		: RunnableSystemWithDecoration
	{
		public Action<String, FluidDynamicSystemStateAbstract> OnComputeCompletedHandler{ get; set; }

		public Action<String, 
			Dictionary<string, GasNodeAbstract>, 
			Dictionary<string, GasEdgeAbstract>, 
			AmbientParameters> OnComputeStartedHandler{ get; set; }

		public RunnableSystemWithDecorationComputeCompletedHandler ()
		{
			OnComputeStartedHandler = (systemName, nodes, edges, ambientParameters) => {};
			OnComputeCompletedHandler = (systemName, systemState) => {};
		}

		#region RunnableSystem implementation
		public override FluidDynamicSystemStateAbstract compute (
			string systemName, 
			Dictionary<string, GasNodeAbstract> nodes, 
			Dictionary<string, GasEdgeAbstract> edges, 
			AmbientParameters ambientParameters)
		{
			OnComputeStartedHandler.Invoke (
				systemName, nodes, edges, ambientParameters);

			FluidDynamicSystemStateAbstract aSystemState = 
				base.compute (systemName, nodes, edges, ambientParameters);

			OnComputeCompletedHandler.Invoke (systemName, aSystemState);
		
			return aSystemState;
		}
		#endregion

	}
}

