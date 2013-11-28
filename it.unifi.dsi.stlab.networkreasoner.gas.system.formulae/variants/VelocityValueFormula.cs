using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class VelocityValueFormula : GasFormulaAbstract<Double>
	{
		public double Qvalue {
			get;
			set;
		}

		public double Diameter {
			get;
			set;
		}

		public double AbsolutePressureOfStartNode {
			get;
			set;
		}

		public double AbsolutePressureOfEndNode {
			get;
			set;
		}
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override Double accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitVelocityValueFormula (this);
		}
		#endregion
	}
}

