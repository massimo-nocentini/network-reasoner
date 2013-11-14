using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithDimension<T> : 
		DimensionalObjectWrapper<T>
	{
		public DimensionalObjectWrapperWithDimension ()
		{
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> makeDimensional (Func<T, T> translator)
		{
			// nothing to do since the wrapped object is already marked as dimensional
			return this;
		}

		public override DimensionalObjectWrapper<T> makeAdimensional (Func<T, T> translator)
		{
			var adimensional = new DimensionalObjectWrapperWithoutDimension<T> ();
			adimensional.WrappedObject = translator.Invoke (this.WrappedObject);
			return adimensional;
		}
		#endregion

	}
}

