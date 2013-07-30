using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class ControVariantLittleKFormula:AbstractLittleKFormula
	{
		public override double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitControVariantLittleKFormula (this);
		}
	}
}

