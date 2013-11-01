using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class ValueHolderCarryInfo<T> : ValueHolder<T>
	{
		public T Value {
			get;
			set;
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.LoadPressureValueHolder
		public override T getValue ()
		{
			return Value;
		}
		#endregion


	}
}

