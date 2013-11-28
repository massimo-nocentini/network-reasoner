using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class AbsolutePressureFromAdimensionalPressureFormulaForNodes:GasFormulaAbstract<double>
	{
		public double AbsolutePressure {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitAbsolutePressureFromAdimensionalPressureFormulaForNodes (this);
		}
		#endregion
	}
}

