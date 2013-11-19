using System;
using System.Collections.Generic;
using System.IO;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;

namespace it.unifi.dsi.stlab.networkreasoner.console.emacs
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			var lines = new List<string> ();
			while (Console.In.Peek() != -1) {
				string input = Console.In.ReadLine ();
				lines.Add (input);
			}

			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("gheonet-textual-networks/five-nodes-network.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted (
					new FileInfo ("gheonet-textual-networks/five-nodes-network-extension.dat"))
			);

			var fiveNodesNetworkRunnableSystem = new FiveNodesNetworkRunnableSystem ();
			systemRunner.run (fiveNodesNetworkRunnableSystem);

			File.WriteAllText ("gheonet-textual-networks/five-nodes-network-output.dat", 
			                  fiveNodesNetworkRunnableSystem.buildTableSummary ());
		}
	}
}
