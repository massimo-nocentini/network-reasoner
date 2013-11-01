using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class SpecificationAssembler
	{
		public abstract SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<ValueHolder<Double>, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationLines,
			TextualGheoNetInputParser parentParser);
	}

}

