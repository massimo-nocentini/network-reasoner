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
			double coVariantLittleKFor (
				EdgeForNetwonRaphsonSystem anEdge);

			double controVariantLittleKFor (
				EdgeForNetwonRaphsonSystem anEdge);

			void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem, double> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
				Vector<NodeForNetwonRaphsonSystem, double> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge);
		}

		/**
		 * We introduce this class since it allow to factor the behavior
		 * for the computation of coVariantLittleKFor and controVariantLittleKFor,
		 * since this behavior doesn't depend on the edge's switch state.
		 */
		public abstract class EdgeStateAbstract : EdgeState
		{
			protected virtual double weightedHeightsDifferenceFor (
				EdgeForNetwonRaphsonSystem anEdge)
			{

				var difference = anEdge.StartNode.Height - anEdge.EndNode.Height;
				var rate = anEdge.AmbientParameters.GravitationalAcceleration / 
					anEdge.AmbientParameters.GasTemperature;

				return rate * difference;

			}
			#region EdgeState implementation
			public virtual double coVariantLittleKFor (
				EdgeForNetwonRaphsonSystem anEdge)
			{
				return anEdge.AmbientParameters.Rconstant + 
					weightedHeightsDifferenceFor(anEdge);
				
			}

			public virtual double controVariantLittleKFor (
				EdgeForNetwonRaphsonSystem anEdge)
			{
				return anEdge.AmbientParameters.Rconstant - 
					weightedHeightsDifferenceFor(anEdge);
			}

			public abstract void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem, double> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
				Vector<NodeForNetwonRaphsonSystem, double> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge);

			#endregion

		}

		public	class EdgeStateOn:EdgeStateAbstract
		{
			#region EdgeState implementation
			public override void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem, double> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
				Vector<NodeForNetwonRaphsonSystem, double> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge)
			{
				var f = Fvector.valueAt (anEdge);
				var A = anEdge.AmbientParameters.Aconstant / Math.Pow (anEdge.Diameter, 5);

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

		public	class EdgeStateOff:EdgeStateAbstract
		{
			#region EdgeState implementation
			public override void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem, double> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
				Vector<NodeForNetwonRaphsonSystem, double> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge)
			{
				// here we don't need to do anything since the edge is switched off.
			}
			#endregion
		}

		public EdgeState SwitchState{ get; set; }

		public long Length { get; set; }

		public double Diameter { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		public double coVariantLittleK ()
		{
			return this.SwitchState.coVariantLittleKFor (this);
		}

		public double controVariantLittleK ()
		{
			return this.SwitchState.controVariantLittleKFor (this);
		}

		public void putKvalueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem, double> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
			Vector<NodeForNetwonRaphsonSystem, double> unknownVector)
		{
			this.SwitchState.putKvalueIntoUsingFor (
				Kvector, Fvector, unknownVector, this);
		}

	}
}

