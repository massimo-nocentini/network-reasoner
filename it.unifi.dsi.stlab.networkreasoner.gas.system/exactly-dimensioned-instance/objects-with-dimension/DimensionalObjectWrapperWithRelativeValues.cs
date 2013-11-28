using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithRelativeValues<T> : 
		DimensionalObjectWrapper<T>
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> makeRelative (Func<T, T> translator)
		{
			// nothing to do since the wrapped object is already marked as dimensional
			return this;
		}

		public override DimensionalObjectWrapper<T> makeAdimensional (Func<T, T> translator)
		{
			var adimensional = new DimensionalObjectWrapperWithAdimensionalValues<T> ();
			adimensional.WrappedObject = translator.Invoke (this.WrappedObject);
			return adimensional;
		}

		public override DimensionalObjectWrapper<T> makeAbsolute (Func<T, T> translator)
		{
			var relative = new DimensionalObjectWrapperWithRelativeValues<T> ();
			relative.WrappedObject = translator.Invoke (this.WrappedObject);
			return relative;
		}
		#endregion

	}
}

