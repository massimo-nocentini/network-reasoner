using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class GasFormulaVisitorExactlyDimensioned : GasFormulaVisitor
	{
		// for ease the implementation we assume that the ambiental
		// parameters are passed from the outside and stored here
		// as an instance variable.
		public AmbientParameters AmbientParameters{ get; set; }

		#region GasFormulaVisitor implementation
		public Double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula)
		{
			var airPressureFormula = new AirPressureFormulaForNodes ();
			airPressureFormula.NodeHeight = aSupplyNodeFormula.NodeHeight;
			var AirPressureInBar = airPressureFormula.accept (this);

			var numerator = aSupplyNodeFormula.GadgetSetupPressureInMillibar / 1000 + 
				AirPressureInBar;

			var denominator = AmbientParameters.RefPressureInBar;

			var Hsetup = Math.Pow (numerator / denominator, 2);
		
			return Hsetup;
		}

		public double visitAirPressureFormulaForNodes (
			AirPressureFormulaForNodes anAirPressureFormula)
		{
			var airPressureInBar = AmbientParameters.AirPressureInBar * 
				Math.Exp (-(AmbientParameters.GravitationalAcceleration * anAirPressureFormula.NodeHeight) / 
				(AmbientParameters.Rconstant * AmbientParameters.AirTemperatureInKelvin)
			);

			return airPressureInBar;
		}

		public double visitRelativePressureFromAbsolutePressureFormulaForNodes (
			RelativePressureFromAbsolutePressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var airPressureFormula = new AirPressureFormulaForNodes ();
			airPressureFormula.NodeHeight = aRelativePressureFromAbsolutePressureFormula.NodeHeight;
			var AirPressureInBar = airPressureFormula.accept (this);

			var result = Math.Sqrt (aRelativePressureFromAbsolutePressureFormula.AbsolutePressure) *
				AmbientParameters.RefPressureInBar;

			return (result - AirPressureInBar) * 1000;
		}

		public double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant + 
				weightedHeightsDifferenceFor (covariantLittleKFormula);
		}

		protected virtual double weightedHeightsDifferenceFor (
				AbstractLittleKFormula abstractLittleKFormula)
		{
			var difference = abstractLittleKFormula.HeightOfStartNode - 
				abstractLittleKFormula.HeightOfEndNode;

			var rate = this.AmbientParameters.GravitationalAcceleration / 
				this.AmbientParameters.GasTemperatureInKelvin;

			return rate * difference;

		}

		public double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant - 
				weightedHeightsDifferenceFor (controVariantLittleKFormula);
		}
		#endregion
	}
}

