using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public interface GasFormulaVisitor
	{

		double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula);

		double visitAirPressureFormulaForNodes (
			AirPressureFormulaForNodes anAirPressureFormula);

		double visitRelativePressureFromAdimensionalPressureFormulaForNodes (
			RelativePressureFromAdimensionalPressureFormulaForNodes aRelativePressureFromAdimensionalPressureFormula);

		double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula);

		double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula);

		double visitKvalueFormula (KvalueFormula KvalueFormula);

		AmatrixQuadruplet visitAmatrixQuadrupletFormulaForSwitchedOnEdges (
			AmatrixQuadrupletFormulaForSwitchedOnEdges 
			amatrixQuadrupletFormulaForSwitchedOnEdges
		);

		AmatrixQuadruplet visitJacobianMatrixQuadrupletFormulaForSwitchedOnEdges (
			JacobianMatrixQuadrupletFormulaForSwitchedOnEdges
			jacobianMatrixQuadrupletFormulaForSwitchedOnEdges
		);

		double visitQvalueFormula (QvalueFormula QvalueFormula);

		double visitFvalueFormula (FvalueFormula FvalueFormula);

		double visitVelocityValueFormula (VelocityValueFormula velocityValueFormula);

		double visitAbsolutePressureFromAdimensionalPressureFormulaForNodes (
			AbsolutePressureFromAdimensionalPressureFormulaForNodes anAbsolutePressureFromAdimensionalPressureFormula);

	}
}

