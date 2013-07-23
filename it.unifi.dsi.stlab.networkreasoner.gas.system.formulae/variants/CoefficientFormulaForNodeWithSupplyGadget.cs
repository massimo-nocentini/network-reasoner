using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class CoefficientFormulaForNodeWithSupplyGadget : GasFormulaAbstract<Double>
	{
		public long NodeHeight {
			get;
			set;
		}

		public double GadgetSetupPressureInMillibar {
			get;
			set;
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override Double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitCoefficientFormulaForNodeWithSupplyGadget (this);
		}
		#endregion

	}
}

