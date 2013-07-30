using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class CovariantLittleKFormula : AbstractLittleKFormula
	{

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override Double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitCovariantLittleKFormula (this);
		}
		#endregion

	}
}

