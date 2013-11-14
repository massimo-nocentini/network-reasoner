using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public abstract class DimensionalObjectWrapper<T>
	{
		public T WrappedObject { get; set; }

		public abstract DimensionalObjectWrapper<T> makeDimensional (
			Func<T, T> translator);

		public abstract DimensionalObjectWrapper<T> makeAdimensional (
			Func<T, T> translator);
	}
}

