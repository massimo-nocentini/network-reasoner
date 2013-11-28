using System;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class AdimensionalPressures : TargetDimension<Vector<NodeForNetwonRaphsonSystem>>
	{
		#region Dimension implementation
		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateRelativeValues (DimensionalObjectWrapperWithRelativeValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithRelativeValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAbsoluteValues (DimensionalObjectWrapperWithAbsoluteValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAbsoluteValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAdimensionalValues (DimensionalObjectWrapperWithAdimensionalValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAdimensionalValues)
		{
			return dimensionalObjectWrapperWithAdimensionalValues;
		}
		#endregion
	}
}

