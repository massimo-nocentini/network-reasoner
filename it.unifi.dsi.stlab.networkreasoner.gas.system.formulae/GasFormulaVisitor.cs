using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public interface GasFormulaVisitor
	{
		double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula);
	}
}

