using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class AmbientParameters
	{
		// for gas
		public String GasName { get; set; }

		public Double MolWeight { get; set; }

		public Double GasTemperatureInKelvin { get; set; }

		public Double ViscosityInPascalTimesSecond { get; set; }

		// ref state
		public Double RefPressureInBar { get; set; }

		public Double RefTemperatureInKelvin { get; set; }

		// air
		public Double AirPressureInBar { get; set; }
	
		public Double AirTemperatureInKelvin { get; set; }

		public Double GravitationalAcceleration {
			get {
				return 9.806;
			}
		}

		public Double Aconstant {
			get {
				var numerator = 16 * GasTemperatureInKelvin;
				var denominator = Math.Pow (Math.PI, 2) * 
					Math.Pow (this.RefTemperatureInKelvin, 2);

				return numerator / denominator;
			}
		}

		public Double Rconstant {
			get {
				return 8314.0 / MolWeight;
			}
		}

		public Double RefDensity ()
		{
			return Math.Pow (10, 5) * RefPressureInBar / 
				(Rconstant * RefTemperatureInKelvin);
		}
	
	}
}

