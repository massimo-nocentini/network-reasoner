using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystem
	{
		public	interface EdgeState
		{
		}

		public	class EdgeStateOn:EdgeState
		{
		}

		public	class EdgeStateOff:EdgeState
		{

		}

		public EdgeState SwitchState{ get; set; }

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

