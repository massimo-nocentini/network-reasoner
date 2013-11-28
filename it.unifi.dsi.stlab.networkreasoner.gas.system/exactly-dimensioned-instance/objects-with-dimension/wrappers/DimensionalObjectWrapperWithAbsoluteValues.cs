using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithAbsoluteValues<T>
		: DimensionalObjectWrapper<T>
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> translateTo (TargetDimension<T> dimension)
		{
			return dimension.translateAbsoluteValues(this);
		}
		#endregion
	}
}

