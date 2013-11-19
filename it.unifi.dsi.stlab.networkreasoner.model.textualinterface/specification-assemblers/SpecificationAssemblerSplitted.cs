using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.extensionmethods;
using System.Globalization;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class SpecificationAssemblerSplitted : SpecificationAssembler
	{
		Lazy<List<String>> LazyNodeDefinitionExtensions{ get; set; }

		public SpecificationAssemblerSplitted (
			FileInfo nodesDefinisionsFileInfo)
		{
			LazyNodeDefinitionExtensions = new Lazy<List<string>> (
				() => {
				var lines = new List<String> (File.ReadLines (nodesDefinisionsFileInfo.FullName));
				var result = new List<String> ();
				lines.RemoveAll (line => line.StartsWith ("|-") || string.IsNullOrEmpty (line));
				lines.ForEach (line => result.Add (line.Trim ()));
				return result;
			}
			);
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.SpecificationAssembler
		public override SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<ValueHolder<Double>, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationsGivenFromParser, 
			TextualGheoNetInputParser parentParser)
		{
			Dictionary<String, SystemRunnerFromTextualGheoNetInput> systems = 
				new Dictionary<string, SystemRunnerFromTextualGheoNetInput> ();

			var headerLine = LazyNodeDefinitionExtensions.Value.First ();

			var columns = parentParser.splitOrgRow (headerLine);

			var columnEnumeration = columns.enumerate ();

			LazyNodeDefinitionExtensions.Value.Rest ().ForEach (
				extensionLine => {

				string[] splittedLine = parentParser.splitOrgRow (extensionLine);

				var nodes = new Dictionary<string, GasNodeAbstract> ();

				nodesSpecificationsGivenFromParser.ForEach (
					aNodeLine => {

					GasNodeAbstract aNode = null;

					if (columnEnumeration.ContainsKey (aNodeLine.Identifier)) {

						// we have to map the column of the node since in the extension
						// file the matrix is transposed, ie the loads/pressures
						// are expressed in column so that each node has a column.
						// Hence from the parsed definition from the parent parser
						// we have to find out where the corresponding extension is.
						var nodeIndex = columnEnumeration [aNodeLine.Identifier];

						var value = parentParser.parseDoubleCultureInvariant (
							splittedLine [nodeIndex]).Value;

						var valueHolder = new ValueHolderCarryInfo<Double> ();
						valueHolder.Value = value;

						aNode = delayedNodesConstruction [aNodeLine.Identifier].
							Invoke (valueHolder);

					} else {

						var valueHolder = new ValueHolderNoInfoShouldBeRequested<Double> ();
						valueHolder.Exception = new Exception (string.Format (
							"No value should be requested because" +
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
			);

			var multipleSystemsRunner = new SystemRunnerFromTextualGheoNetInputMultipleSystems ();
			multipleSystemsRunner.Systems = systems;

			return multipleSystemsRunner;

		}
		#endregion


	}
}

