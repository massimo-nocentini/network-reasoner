using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class GasFormulaVisitorExactlyDimensionedForWater : GasFormulaVisitorExactlyDimensioned
	{
		public override double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aSupplyNodeFormula);

			var specWeight = AmbientParameters.RefDensity () * AmbientParameters.GravitationalAcceleration;

			// pressione assoluta al nodo espressa in metri
			var numerator = (AirPressureInBar + 
				aSupplyNodeFormula.GadgetSetupPressureInMillibar) * 1e5 // per andare in pascal
				/ specWeight;

			var result = numerator + aSupplyNodeFormula.NodeHeight;
		
			return result;
		}

		public override double visitRelativePressureFromAdimensionalPressureFormulaForNodes (
			RelativePressureFromAdimensionalPressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aRelativePressureFromAbsolutePressureFormula);

			var z = aRelativePressureFromAbsolutePressureFormula.AbsolutePressure;
			var h = aRelativePressureFromAbsolutePressureFormula.NodeHeight;
					
			var specWeight = AmbientParameters.RefDensity () * 
				AmbientParameters.GravitationalAcceleration;

			var result = (z - h) * specWeight * 1e-5;

			return (result - AirPressureInBar);
		}

		public override double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula)
		{
			return 1;
		}

		public override double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula)
		{
			return 1;
		}

		public override double visitVelocityValueFormula (VelocityValueFormula velocityValueFormula)
		{
			double Qvalue = velocityValueFormula.Qvalue;
			double diameter = velocityValueFormula.Diameter;
			double absolutePressureOfStartNode = velocityValueFormula.AbsolutePressureOfStartNode;
			double absolutePressureOfEndNode = velocityValueFormula.AbsolutePressureOfEndNode;
		
			var numerator = Qvalue / 3600d;
			var denominator = Math.PI * Math.Pow (diameter * 1e-3, 2d) / 4d; 

			return numerator / denominator;
			
		}
	}
}

