using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class SpecificationAssemblerAllInOneFile : SpecificationAssembler
	{

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.SpecificationAssembler
		public override SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<ValueHolder<Double>, GasNodeAbstract>> delayedNodesConstruction, 
			List<NodeSpecificationLine> nodesSpecificationLines,
			TextualGheoNetInputParser parentParser)
		{			
			Dictionary<string, GasNodeAbstract> nodesDictionary = 
				new Dictionary<string, GasNodeAbstract> ();

			nodesSpecificationLines.ForEach (nodeSpecification => {

				var ctorFunction = delayedNodesConstruction [nodeSpecification.Identifier];

				var supplyValue = parentParser.parseDoubleCultureInvariant (
					nodeSpecification.SplittedSpecification [3]).Value;
				var loadValue = parentParser.parseDoubleCultureInvariant (
					nodeSpecification.SplittedSpecification [2]).Value;

				var value = nodeSpecification.Type == NodeType.WithSupplyGadget ?
					supplyValue : loadValue;

				var valueHolder = new ValueHolderCarryInfo<Double> ();
				valueHolder.Value = value;

				nodesDictionary.Add (nodeSpecification.Identifier, 
				                     ctorFunction.Invoke (valueHolder));
			}
			);

			var edgesDictionary = parentParser.parseEdgesRelating (nodesDictionary);

			var ambientParameters = parentParser.parseAmbientParameters ();

			return new SystemRunnerFromTextualGheoNetInputSingleSystem{
				SystemName = "single-system",
				Nodes = nodesDictionary,
				Edges = edgesDictionary,
				AmbientParameters = ambientParameters
			};
				
		}
		#endregion

	}
}

