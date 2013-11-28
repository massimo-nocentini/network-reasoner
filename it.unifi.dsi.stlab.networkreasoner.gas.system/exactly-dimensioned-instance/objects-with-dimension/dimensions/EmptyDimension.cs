using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class EmptyDimension<T> : TargetDimension<T>
	{
		#region Dimension implementation
		public DimensionalObjectWrapper<T> translateRelativeValues (DimensionalObjectWrapperWithRelativeValues<T> dimensionalObjectWrapperWithRelativeValues)
		{
			throw makeExceptionForSource ("relative");
		}

		public DimensionalObjectWrapper<T> translateAbsoluteValues (DimensionalObjectWrapperWithAbsoluteValues<T> dimensionalObjectWrapperWithAbsoluteValues)
		{
			throw makeExceptionForSource ("absolute");
		}

		public DimensionalObjectWrapper<T> translateAdimensionalValues (DimensionalObjectWrapperWithAdimensionalValues<T> dimensionalObjectWrapperWithAdimensionalValues)
		{
			throw makeExceptionForSource ("adimensional");
		}
		#endregion

		protected virtual Exception makeExceptionForSource (string sourceDimension)
		{
			return new Exception (string.Format ("No rules for translate relative " +
				"values of type {0} to target dimension of type {1}",
			                                   sourceDimension,
			                                   typeof(T).FullName)
			);
		}
	}
}

