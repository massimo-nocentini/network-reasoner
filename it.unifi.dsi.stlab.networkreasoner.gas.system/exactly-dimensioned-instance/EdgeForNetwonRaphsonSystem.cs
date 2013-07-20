using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	internal class EdgeForNetwonRaphsonSystem
	{
		public long Length { get; set; }

		public double Diameter { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		public double coVariantLittleK ()
		{
			return AmbientParameters.Rconstant + weightedHeightsDifference;
		}

		public double controVariantLittleK ()
		{
			return AmbientParameters.Rconstant - weightedHeightsDifference;
		}

		protected virtual double weightedHeightsDifference {
			get {
				var difference = StartNode.Height - EndNode.Height;
				var rate = AmbientParameters.GravitationalAcceleration / 
					AmbientParameters.GasTemperature;
				return rate * difference;
			}
		}


	}
}

