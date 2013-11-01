using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class ValueHolderNoInfoShouldBeRequested<T> : ValueHolder<T>
	{
		public Exception Exception {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.LoadPressureValueHolder
		public override T getValue ()
		{
			throw Exception;
		}
		#endregion
	}
}

