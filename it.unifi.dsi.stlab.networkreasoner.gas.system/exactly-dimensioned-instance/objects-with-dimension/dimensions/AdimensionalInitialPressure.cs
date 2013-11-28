using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class AdimensionalInitialPressure : TargetDimension<Double>
	{
		#region Dimension implementation
		public DimensionalObjectWrapper<double> translateRelativeValues (DimensionalObjectWrapperWithRelativeValues<double> dimensionalObjectWrapperWithRelativeValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<double> translateAbsoluteValues (DimensionalObjectWrapperWithAbsoluteValues<double> dimensionalObjectWrapperWithAbsoluteValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<double> translateAdimensionalValues (DimensionalObjectWrapperWithAdimensionalValues<double> dimensionalObjectWrapperWithAdimensionalValues)
		{
			return dimensionalObjectWrapperWithAdimensionalValues;
		}
		#endregion
	}
}

