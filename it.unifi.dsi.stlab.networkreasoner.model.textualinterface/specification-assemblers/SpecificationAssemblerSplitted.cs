using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.extensionmethods;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class SpecificationAssemblerSplitted : SpecificationAssembler
	{
		string NodesDefinisionsFilename{ get; set; }

		public SpecificationAssemblerSplitted (
			string nodesDefinisionsFilename)
		{
			NodesDefinisionsFilename = nodesDefinisionsFilename;
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.SpecificationAssembler
		public override SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<double, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationLines, 
			TextualGheoNetInputParser parentParser)
		{
			var nodesLines = new List<String> (File.ReadLines (NodesDefinisionsFilename));

			Dictionary<String, SystemRunnerFromTextualGheoNetInput> multipleSystems = 
				new Dictionary<string, SystemRunnerFromTextualGheoNetInput> ();

			var headerLine = nodesLines [0];

			var columns = this.SplitLineOnTabs (headerLine);

			var columnEnumeration = columns.enumerate ();

			// we start from 1 in order to ignore the header line
			for (int i = 1; i < nodesLines.Count; i = i + 1) {

				string[] splittedLine = this.SplitLineOnTabs (nodesLines [i]);

				var nodes = new Dictionary<string, GasNodeAbstract> ();

				nodesSpecificationLines.ForEach (
					aNodeLine => {

					GasNodeAbstract aNode = null;

					if (columnEnumeration.ContainsKey (aNodeLine.Identifier)) {

						var nodeIndex = columnEnumeration [aNodeLine.Identifier];

						var value = Double.Parse (splittedLine [nodeIndex]);

						aNode = delayedNodesConstruction [aNodeLine.Identifier].Invoke (value);

					} else {
						// here we give NaN in order to ease debugging because if the
						// headers do not contain a node present in the given delay construction dictionary
						// so the value for that node have to be handled in the parent parser code (
						// in other word the Double.NaN value should be ignored).
						aNode = delayedNodesConstruction [aNodeLine.Identifier].Invoke (Double.NaN);
					}

					nodes.Add (aNodeLine.Identifier, aNode);
				}
				);

				var edges = parentParser.parseEdgesRelating (nodes);

				var ambientParameters = parentParser.parseAmbientParameters ();

				var systemName = splittedLine [0];

				multipleSystems.Add (systemName, new SystemRunnerFromTextualGheoNetInputSingleSystem{
					SystemName = systemName,
					AmbientParameters = ambientParameters,
					Edges = edges,
					Nodes = nodes
				}
				);

			}

			var multipleSystemsRunner = new SystemRunnerFromTextualGheoNetInputMultipleSystems ();
			multipleSystemsRunner.Systems = multipleSystems;

			return multipleSystemsRunner;

		}
		#endregion

		protected virtual String[] SplitLineOnTabs (String toSplit)
		{
			return toSplit.Split (new char[]{'\t'}, StringSplitOptions.RemoveEmptyEntries);
		}


	}
}

