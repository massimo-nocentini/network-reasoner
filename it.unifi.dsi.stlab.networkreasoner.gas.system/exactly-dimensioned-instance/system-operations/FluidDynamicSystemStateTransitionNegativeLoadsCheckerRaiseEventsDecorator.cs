using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator :
		FluidDynamicSystemStateTransitionNegativeLoadsChecker
	{
		public NetwonRaphsonSystemEventsListener EventsListener{ get; set; }

		protected override DimensionalObjectWrapper<
			Vector<NodeForNetwonRaphsonSystem>> makeUnknownsDimensional (
				OneStepMutationResults mutationResult)
		{
			var dimensionalUnknowns = base.makeUnknownsDimensional (mutationResult);
			
			EventsListener.onUnknownWithDimensionReverted (
				mutationResult.StartingUnsolvedState.NodesEnumeration, 
				dimensionalUnknowns.WrappedObject);

			return dimensionalUnknowns;

		}
	}
}

