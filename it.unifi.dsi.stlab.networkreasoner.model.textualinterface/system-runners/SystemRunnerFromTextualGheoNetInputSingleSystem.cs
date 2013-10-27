using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	class SystemRunnerFromTextualGheoNetInputSingleSystem :
		SystemRunnerFromTextualGheoNetInput
	{
		public String SystemName{ get; set; }

		public Dictionary<string, GasNodeAbstract> Nodes{ get; set; }

		public Dictionary<string, GasEdgeAbstract> Edges{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.SystemRunnerFromTextualGheoNetInput
		public override void run (RunnableSystem runnableSystem)
		{
			runnableSystem.compute (SystemName, Nodes, Edges, AmbientParameters);
		}
		#endregion

	}

}

