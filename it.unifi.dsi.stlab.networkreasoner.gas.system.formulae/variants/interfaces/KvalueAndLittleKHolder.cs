using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public interface KvalueAndLittleKHolder
	{
		double EdgeKvalue {
			get;
			set;
		}

		double EdgeCovariantLittleK {
			get;
			set;
		}

		double EdgeControVariantLittleK {
			get;
			set;
		}
	}
}

