using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class AirPressureFormulaForNodes: GasFormulaAbstract<Double>
	{
		public long NodeHeight {
			get;
			set;
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitAirPressureFormulaForNodes (this);
		}
		#endregion

	}
}

