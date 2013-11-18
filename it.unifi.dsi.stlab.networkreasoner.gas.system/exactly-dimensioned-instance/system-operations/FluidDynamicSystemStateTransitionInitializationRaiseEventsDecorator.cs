using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator :
		FluidDynamicSystemStateTransitionInitialization
	{
		public NetwonRaphsonSystemEventsListener EventsListener{ get; set; }

		public override FluidDynamicSystemStateAbstract forBareSystemState (
			FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			var abstractState = base.forBareSystemState (fluidDynamicSystemStateBare);
			if (abstractState is FluidDynamicSystemStateUnsolved) {

				var unsolvedState = abstractState as FluidDynamicSystemStateUnsolved;

				EventsListener.onInitializationCompleted (
					unsolvedState.Nodes, 
					unsolvedState.Edges, 
					unsolvedState.NodesEnumeration);
			} else {
				// TODO this else branch has to be removed at the end of the refactoring
				throw new Exception ("Massimo's mistake: check the creation of unsolved state after initialization transition");
			}

			return abstractState;
		}

		public override FluidDynamicSystemStateTransition clone ()
		{
			var baseClone = base.clone () as FluidDynamicSystemStateTransitionInitialization;
			var clone = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
			clone.FromDimensionalToAdimensionalTranslator = baseClone.FromDimensionalToAdimensionalTranslator;
			clone.Network = baseClone.Network;
			clone.UnknownInitialization = baseClone.UnknownInitialization;
			clone.EventsListener = this.EventsListener;
			return clone;
		}
	}
}

