using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class AmatrixQuadrupletFormulaForSwitchedOnEdges : 
		GasFormulaAbstract<AmatrixQuadruplet>
	{
		public double EdgeKvalue {
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

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.formulae.GasFormulaAbstract
		public override AmatrixQuadruplet accept (GasFormulaVisitor aVisitor)
		{
			return aVisitor.visitAmatrixQuadrupletFormulaForSwitchedOnEdges (this);
		}
		#endregion
	}
}

