using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public interface GasFormulaVisitor
	{

		double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula);

		double visitAirPressureFormulaForNodes (
			AirPressureFormulaForNodes anAirPressureFormula);

		double visitRelativePressureFromAbsolutePressureFormulaForNodes (
			RelativePressureFromAbsolutePressureFormulaForNodes 
			aRelativePressureFromAbsolutePressureFormula
		);

		double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula);

		double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula);

		double visitKvalueFormula (
			KvalueFormula kvalueFormula);

		AmatrixQuadruplet visitAmatrixQuadrupletFormulaForSwitchedOnEdges (
			AmatrixQuadrupletFormulaForSwitchedOnEdges 
			amatrixQuadrupletFormulaForSwitchedOnEdges
		);

		AmatrixQuadruplet visitJacobianMatrixQuadrupletFormulaForSwitchedOnEdges (
			JacobianMatrixQuadrupletFormulaForSwitchedOnEdges
			jacobianMatrixQuadrupletFormulaForSwitchedOnEdges
		);
	}
}

