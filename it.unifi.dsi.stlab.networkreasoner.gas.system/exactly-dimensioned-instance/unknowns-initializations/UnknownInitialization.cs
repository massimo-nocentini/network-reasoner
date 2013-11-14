using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class UnknownInitialization
	{
		public abstract double initialValueFor (GasNodeAbstract aVertex, Random rand);
	}

}

