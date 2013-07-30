using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class FvalueFormula : GasFormulaAbstract<Double>
	{
		public double EdgeQvalue {
			get;
			set;
		}

		public double EdgeDiameterInMillimeters {
			get;
			set;
		}

		public double EdgeRoughnessInMicron {
			get;
			set;
		}

		public double EdgeFvalue {
			get;
			set;
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override Double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitFvalueFormula (this);
		}
		#endregion
	
	}
}

