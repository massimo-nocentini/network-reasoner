using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public abstract class GasFormulaAbstract<ReturnType>
	{
		public abstract ReturnType accept (GasFormulaVisitor aVisitor);
	}
}

