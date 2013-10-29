using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class SpecificationAssembler
	{
		public abstract SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<LoadPressureValueHolder, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationLines,
			TextualGheoNetInputParser parentParser);
	}

}

