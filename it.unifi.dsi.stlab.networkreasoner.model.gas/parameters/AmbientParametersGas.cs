using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class AmbientParametersGas : AmbientParameters
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters
		public override double Aconstant ()
		{
			var numerator = 16 * ElementTemperatureInKelvin;
			var denominator = Math.Pow (Math.PI, 2) * 
				Math.Pow (this.RefTemperatureInKelvin, 2);

			return numerator / denominator;		
		}
		
		public override double RefDensity ()
		{		
			return Math.Pow (10, 5) * RefPressureInBar / 
				(Rconstant () * RefTemperatureInKelvin);		
		}
		#endregion


	}
}

