using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class AmbientParameters
	{
		// for gas
		public String ElementName { get; set; }

		public Double MolWeight { get; set; }

		public Double ElementTemperatureInKelvin { get; set; }

		public Double ViscosityInPascalTimesSecond { get; set; }

		// ref state
		public Double RefPressureInBar { get; set; }

		public Double RefTemperatureInKelvin { get; set; }

		// air
		public Double AirPressureInBar { get; set; }
	
		public Double AirTemperatureInKelvin { get; set; }

		public Double GravitationalAcceleration {
			get {
				return 9.806d;
			}
		}

		public abstract Double Aconstant ();

		public abstract Double RefDensity ();

		public virtual Double Rconstant ()
		{
			return 8314d / MolWeight;
			
		}



	
	}
}

