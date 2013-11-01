using System;

namespace it.unifi.dsi.stlab.utilities.value_holders
{
	public class ValueHolderCarryInfo<T> : ValueHolder<T>
	{
		public T Value {
			get;
			set;
		}
		
		public override T getValue ()
		{
			return Value;
		}
		


	}
}

