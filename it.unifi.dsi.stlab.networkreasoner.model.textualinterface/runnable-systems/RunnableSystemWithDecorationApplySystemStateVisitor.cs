using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class RunnableSystemWithDecorationApplySystemStateVisitor : 
		RunnableSystemWithDecoration
	{
		public FluidDynamicSystemStateVisitorWithSystemName SystemStateVisitor { get; set; }

		#region RunnableSystem implementation
		public override FluidDynamicSystemStateAbstract compute (
			string systemName, 
			Dictionary<string, GasNodeAbstract> nodes, 
			Dictionary<string, GasEdgeAbstract> edges, 
			AmbientParameters ambientParameters)
		{

			FluidDynamicSystemStateAbstract aSystemState =
				base.compute (systemName, nodes, edges, ambientParameters);

			SystemStateVisitor.SystemName = systemName;
			aSystemState.accept (SystemStateVisitor);
		
			return aSystemState;
		}
		#endregion
	}
}

