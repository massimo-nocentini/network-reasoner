using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class AmbientParametersWater : AmbientParameters
	{
		// per i k -piccoli tutti uguali a 1 per ogni nodo

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters
		public override double Aconstant ()
		{
			var numerator = 8.0;
			var denominator = this.GravitationalAcceleration *
				Math.Pow (Math.PI, 2);

			return numerator / denominator;
		}

		public override double RefDensity ()
		{
			return 1000; // kg/m^3
		}
		#endregion
	}
}

