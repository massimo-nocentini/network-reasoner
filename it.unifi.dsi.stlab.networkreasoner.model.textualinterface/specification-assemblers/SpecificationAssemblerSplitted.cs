using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.extensionmethods;
using System.Globalization;

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
			Dictionary<string, Func<LoadPressureValueHolder, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationLines, 
			TextualGheoNetInputParser parentParser)
		{
			var nodesLines = new List<String> (File.ReadLines (NodesDefinisionsFilename));

			Dictionary<String, SystemRunnerFromTextualGheoNetInput> systems = 
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

						var value = Double.Parse (splittedLine [nodeIndex], CultureInfo.InvariantCulture);

						var valueHolder = new LoadPressureValueHolderCarryInfo ();
						valueHolder.Value = value;

						aNode = delayedNodesConstruction [aNodeLine.Identifier].Invoke (valueHolder);

					} else {

						var valueHolder = new LoadPressureValueHolderNoInfoShouldBeRequested ();
						valueHolder.Exception = new Exception (string.Format ("No value should be requested because" +
							" the definition says that node {0} is a passive" +
							" node, so it has load 0, hence this value request is meaningless.",
						                                                     aNodeLine.Identifier)
						);

						aNode = delayedNodesConstruction [aNodeLine.Identifier].Invoke (valueHolder);
					}

					nodes.Add (aNodeLine.Identifier, aNode);
				}
				);

				var edges = parentParser.parseEdgesRelating (nodes);

				var ambientParameters = parentParser.parseAmbientParameters ();

				var systemName = splittedLine [0];

				systems.Add (systemName, new SystemRunnerFromTextualGheoNetInputSingleSystem{
					SystemName = systemName,
					AmbientParameters = ambientParameters,
					Edges = edges,
					Nodes = nodes
				}
				);

			}

			var multipleSystemsRunner = new SystemRunnerFromTextualGheoNetInputMultipleSystems ();
			multipleSystemsRunner.Systems = systems;

			return multipleSystemsRunner;

		}
		#endregion

		protected virtual String[] SplitLineOnTabs (String toSplit)
		{
			return toSplit.Split (new char[]{'\t'}, StringSplitOptions.RemoveEmptyEntries);
		}


	}
}

