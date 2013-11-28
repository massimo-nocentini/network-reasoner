using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class RunnableSystemComputeGivenEventListener : RunnableSystemCompute
	{
		public NetwonRaphsonSystemEventsListener EventListener { get; set; }

		protected override NetwonRaphsonSystemEventsListener buildEventListener ()
		{
			return EventListener;
		}
	}
}

