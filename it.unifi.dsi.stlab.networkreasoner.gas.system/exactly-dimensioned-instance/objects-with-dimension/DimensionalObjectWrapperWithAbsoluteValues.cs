using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class DimensionalObjectWrapperWithAbsoluteValues<T>
		: DimensionalObjectWrapper<T>
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> makeRelative (Func<T, T> translator)
		{
			var relative = new DimensionalObjectWrapperWithRelativeValues<T> ();
			relative.WrappedObject = translator.Invoke (this.WrappedObject);
			return relative;
		}

		public override DimensionalObjectWrapper<T> makeAbsolute (Func<T, T> translator)
		{
			return this;
		}

		public override DimensionalObjectWrapper<T> makeAdimensional (Func<T, T> translator)
		{
			var adimensional = new DimensionalObjectWrapperWithAdimensionalValues<T> ();
			adimensional.WrappedObject = translator.Invoke (this.WrappedObject);
			return adimensional;
		}
		#endregion
	}
}

