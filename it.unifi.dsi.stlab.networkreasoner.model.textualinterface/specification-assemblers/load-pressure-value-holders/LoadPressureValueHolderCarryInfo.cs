using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class LoadPressureValueHolderCarryInfo : LoadPressureValueHolder
	{
		public double Value {
			get;
			set;
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.LoadPressureValueHolder
		public override double getValue ()
		{
			return Value;
		}
		#endregion


	}
}

