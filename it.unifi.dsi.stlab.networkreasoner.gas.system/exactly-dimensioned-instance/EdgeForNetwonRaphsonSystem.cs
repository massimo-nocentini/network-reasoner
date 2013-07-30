using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystem
	{
		public	interface EdgeState
		{
			void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge);
		}

		public	class EdgeStateOn:EdgeState
		{
			#region EdgeState implementation
			public  void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge)
			{
				var f = Fvector.valueAt (anEdge);
				var A = anEdge.AmbientParameters.Aconstant / Math.Pow (anEdge.DiameterInMillimeters, 5);

				var unknownForStartNode = unknownVector.valueAt (anEdge.StartNode);
				var unknownForEndNode = unknownVector.valueAt (anEdge.EndNode);
				
				var weightedHeightsDifference = 
					anEdge.coVariantLittleK () * unknownForStartNode - 
					anEdge.controVariantLittleK () * unknownForEndNode;

				var K = 1 / Math.Sqrt (f * A * anEdge.Length * weightedHeightsDifference);

				Kvector.atPut (anEdge, K);
			}
			#endregion
		}

		public	class EdgeStateOff:EdgeState
		{
			#region EdgeState implementation
			public  void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge)
			{
				// here we don't need to do anything since the edge is switched off.
			}
			#endregion
		}

		public EdgeState SwitchState{ get; set; }

		public long Length { get; set; }

		public double DiameterInMillimeters { get; set; }

		public double RoughnessInMicron { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		protected virtual double weightedHeightsDifferenceFor (
				EdgeForNetwonRaphsonSystem anEdge)
		{

			var difference = anEdge.StartNode.Height - anEdge.EndNode.Height;
			var rate = anEdge.AmbientParameters.GravitationalAcceleration / 
				anEdge.AmbientParameters.GasTemperatureInKelvin;

			return rate * difference;

		}

		public double coVariantLittleK ()
		{
			return this.AmbientParameters.Rconstant + 
				weightedHeightsDifferenceFor (this);
		}

		public double controVariantLittleK ()
		{
			return this.AmbientParameters.Rconstant - 
				weightedHeightsDifferenceFor (this);
		}

		public void putKvalueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector)
		{
			this.SwitchState.putKvalueIntoUsingFor (
				Kvector, Fvector, unknownVector, this);
		}

	}
}

