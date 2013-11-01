using System;

namespace it.unifi.dsi.stlab.utilities.value_holders
{
	public class ValueHolderNoInfoShouldBeRequested<T> : ValueHolder<T>
	{
		public Exception Exception {
			get;
			set;
		}

		public ValueHolderNoInfoShouldBeRequested ()
		{
			this.Exception = new Exception ("Default message: a value has " +
				"been requested but this value holder carries no value at all because the " +
				"context in which this object has been created do not need to provide any value."
			);
		}

		public override T getValue ()
		{
			throw Exception;
		}
		
	}
}

