using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class KvalueFormula : GasFormulaAbstract<Double>
	{
		public double EdgeFvalue {
			get;
			set;
		}

		public double EdgeDiameterInMillimeters {
			get;
			set;
		}

		public double UnknownForEdgeStartNode {
			get;
			set;
		}

		public double UnknownForEdgeEndNode {
			get;
			set;
		}

		public double EdgeCovariantLittleK {
			get;
			set;
		}

		public double EdgeControVariantLittleK {
			get;
			set;
		}

		public double EdgeLength {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override Double accept (
			GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitKvalueFormula (this);
		}
		#endregion

	}
}

