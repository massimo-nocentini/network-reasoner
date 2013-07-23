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
			var AirPressureInBar = AmbientParameters.AirPressureInBar * 
				Math.Exp (-(AmbientParameters.GravitationalAcceleration * aSupplyNodeFormula.NodeHeight) / 
				(AmbientParameters.Rconstant * AmbientParameters.AirTemperatureInKelvin)
			);

			var Hsetup = Math.Pow (((aSupplyNodeFormula.GadgetSetupPressureInMillibar / 1000 + AirPressureInBar) / 
				AmbientParameters.RefPressureInBar), 2);
		
			return Hsetup;
		}
		#endregion
	}
}

