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

		public Double Aconstant {
			get {
				throw new NotImplementedException ("The ``A'' constant should be calculated using the above fields.");
			}
		}

		public AmbientParameters ()
		{
		}
	}
}

