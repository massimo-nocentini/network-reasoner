using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.extensionmethods;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	class SystemRunnerFromTextualGheoNetInputMultipleSystems : SystemRunnerFromTextualGheoNetInput
	{
		public Dictionary<string, SystemRunnerFromTextualGheoNetInput> Systems {
			get;
			set;
		}

		public override void run (RunnableSystem runnableSystem)
		{
			foreach (var pair in this.Systems) {
				pair.Value.run (runnableSystem);
			}
		}

	}

}

