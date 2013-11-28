using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public abstract class DimensionalObjectWrapper<T>
	{
		public T WrappedObject { get; set; }

		public abstract DimensionalObjectWrapper<T> translateTo (TargetDimension<T> dimension);
	}
}

