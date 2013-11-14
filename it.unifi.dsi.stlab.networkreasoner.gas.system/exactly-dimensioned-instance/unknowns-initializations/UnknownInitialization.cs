using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations
{
	public abstract class UnknownInitialization
	{
		public abstract double initialValueFor (GasNodeAbstract aVertex, Random rand);
	}

}

