using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public abstract class AbstractLittleKFormula : GasFormulaAbstract<Double>
	{
		public long HeightOfStartNode {
			get;
			set;
		}

		public long HeightOfEndNode {
			get;
			set;
		}
	}
}

