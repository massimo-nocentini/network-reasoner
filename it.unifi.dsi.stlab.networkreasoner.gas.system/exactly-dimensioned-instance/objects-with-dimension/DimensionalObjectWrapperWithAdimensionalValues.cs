using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithAdimensionalValues<T> :
		DimensionalObjectWrapper<T>
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> makeRelative (Func<T, T> translator)
		{
			var relative = new DimensionalObjectWrapperWithRelativeValues<T> ();
			relative.WrappedObject = translator.Invoke (this.WrappedObject);
			return relative;
		}

		public override DimensionalObjectWrapper<T> makeAdimensional (Func<T, T> translator)
		{
			// nothing to do since the wrapped object is already marked as adimensional
			return this;
		}

		public override DimensionalObjectWrapper<T> makeAbsolute (Func<T, T> translator)
		{
			var absolute = new DimensionalObjectWrapperWithAbsoluteValues<T> ();
			absolute.WrappedObject = translator.Invoke (this.WrappedObject);
			return absolute;
		}
		#endregion


	}
}

