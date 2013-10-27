using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class RunnableSystemAbstractComputationalResultHandler : RunnableSystem
	{
		#region RunnableSystem implementation
		public abstract void compute (
			string systemName, 
			System.Collections.Generic.Dictionary<string, it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeAbstract> nodes, 
			System.Collections.Generic.Dictionary<string, it.unifi.dsi.stlab.networkreasoner.model.gas.GasEdgeAbstract> edges, 
			it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters);
		#endregion

		protected abstract void onComputationFinished (
			String systemName, OneStepMutationResults results);
	}
}

