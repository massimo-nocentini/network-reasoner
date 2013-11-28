using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithAdimensionalValues<T> :
		DimensionalObjectWrapper<T>
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> translateTo (TargetDimension<T> dimension)
		{
			return dimension.translateAdimensionalValues(this);
		}
		#endregion



	}
}

