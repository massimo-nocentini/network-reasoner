using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class AmbientParameters
	{
		public String GasName { get; set; }

		public Double RefPressure { get; set; }

		public Double AirPressure { get; set; }

		public Double AirTemperature { get; set; }

		public Double GasTemperature { get; set; }

		public Double MolWeight { get; set; }

		public Double Viscosity { get; set; }

		public Double GravitationalAcceleration {
			get {
				return 9.806;
			}
		}

		public Double Aconstant {
			get {
				var numerator = 16 * GasTemperature;
				var denominator = Math.Pow (Math.PI, 2) * 
					Math.Pow (AirTemperature, 2);

				return numerator / denominator;
			}
		}

		public Double Rconstant {
			get {
				return 8314.0 / MolWeight;
			}
		}

		public AmbientParameters ()
		{
		}
	}
}

