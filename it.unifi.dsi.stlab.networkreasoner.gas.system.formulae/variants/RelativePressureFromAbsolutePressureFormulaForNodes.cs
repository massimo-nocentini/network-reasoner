using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class RelativePressureFromAbsolutePressureFormulaForNodes:GasFormulaAbstract<double>
	{
		public long NodeHeight {
			get;
			set;
		}

		public double AbsolutePressure {
			get;
			set;
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitRelativePressureFromAbsolutePressureFormulaForNodes (this);
		}
		#endregion
	}
}

