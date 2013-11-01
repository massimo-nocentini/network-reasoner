using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class LoadPressureValueHolderNoInfoShouldBeRequested : LoadPressureValueHolder
	{
		public Exception Exception {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.LoadPressureValueHolder
		public override double getValue ()
		{
			throw Exception;
		}
		#endregion
	}
}

