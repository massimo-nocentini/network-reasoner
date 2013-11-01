using System;

namespace it.unifi.dsi.stlab.utilities.value_holders
{
	public class ValueHolderNoInfoShouldBeRequested<T> : ValueHolder<T>
	{
		public Exception Exception {
			get;
			set;
		}
		
		public override T getValue ()
		{
			throw Exception;
		}
		
	}
}

