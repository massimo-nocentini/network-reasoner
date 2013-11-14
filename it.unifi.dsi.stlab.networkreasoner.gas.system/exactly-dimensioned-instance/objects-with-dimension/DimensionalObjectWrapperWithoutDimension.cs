using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalObjectWrapperWithoutDimension<T> :
		DimensionalObjectWrapper<T>
	{
		public DimensionalObjectWrapperWithoutDimension ()
		{
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.DimensionalObjectWrapper
		public override DimensionalObjectWrapper<T> makeDimensional (Func<T, T> translator)
		{
			var dimensional = new DimensionalObjectWrapperWithDimension<T> ();
			dimensional.WrappedObject = translator.Invoke (this.WrappedObject);
			return dimensional;
		}

		public override DimensionalObjectWrapper<T> makeAdimensional (Func<T, T> translator)
		{
			// nothing to do since the wrapped object is already marked as adimensional
			return this;
		}
		#endregion

	}
}

