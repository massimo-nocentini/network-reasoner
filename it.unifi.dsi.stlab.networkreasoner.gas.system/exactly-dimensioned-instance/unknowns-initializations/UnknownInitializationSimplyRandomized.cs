using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations
{
	public class UnknownInitializationSimplyRandomized : UnknownInitialization
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.UnknownInitialization
		public override  DimensionalObjectWrapper<double> initialValueFor (GasNodeAbstract aVertex, Random rand)
		{
			var initialValue = (rand.NextDouble () * .1) + 1;
		
			return new DimensionalObjectWrapperWithoutDimension<double>{
				WrappedObject = initialValue
			};
		}
		#endregion

	}
}

