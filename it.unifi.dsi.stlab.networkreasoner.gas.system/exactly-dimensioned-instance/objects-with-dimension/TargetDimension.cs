using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public interface TargetDimension<T>
	{
		DimensionalObjectWrapper<T> translateRelativeValues (
			DimensionalObjectWrapperWithRelativeValues<T> dimensionalObjectWrapperWithRelativeValues);

		DimensionalObjectWrapper<T> translateAbsoluteValues (
			DimensionalObjectWrapperWithAbsoluteValues<T> dimensionalObjectWrapperWithAbsoluteValues);

		DimensionalObjectWrapper<T> translateAdimensionalValues (
			DimensionalObjectWrapperWithAdimensionalValues<T> dimensionalObjectWrapperWithAdimensionalValues);
	}
}

