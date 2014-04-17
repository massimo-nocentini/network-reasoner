using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class RelativePressureFromAdimensionalPressureFormulaForNodes:GasFormulaAbstract<double>, NodeHeightHolder
	{
		public long NodeHeight {
			get;
			set;
		}

		public double AdimensionalPressure {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitRelativePressureFromAdimensionalPressureFormulaForNodes (this);
		}
		#endregion
	}
}

