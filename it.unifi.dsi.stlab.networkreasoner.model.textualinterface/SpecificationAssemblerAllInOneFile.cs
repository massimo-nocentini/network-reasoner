using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class SpecificationAssemblerAllInOneFile : SpecificationAssembler
	{
		public SpecificationAssemblerAllInOneFile ()
		{
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.SpecificationAssembler
		public override SystemRunnerFromTextualGheoNetInput assemble (
			Dictionary<string, Func<double, GasNodeAbstract>> delayedNodesConstruction, 
			List<string> nodesSpecificationLines,
			TextualGheoNetInputParser parentParser)
		{			
			Dictionary<string, GasNodeAbstract> nodesDictionary = 
				new Dictionary<string, GasNodeAbstract> ();

			nodesSpecificationLines.ForEach (nodeSpecification => {

				var splittedSpecification = nodeSpecification.Split (' ');

				var nodeIdentifier = splittedSpecification [0];

				var ctorFunction = delayedNodesConstruction [nodeIdentifier];

				var value = splittedSpecification [1].Equals ("1") ?
					Double.Parse (splittedSpecification [3]) :
						Double.Parse (splittedSpecification [2]);

				nodesDictionary.Add (nodeIdentifier, ctorFunction.Invoke (value));
			}
			);

			var edgesDictionary = parentParser.parseEdgesRelating (nodesDictionary);

			var ambientParameters = parentParser.parseAmbientParameters ();

			return new SystemRunnerFromTextualGheoNetInputSingleSystem{
				Nodes = nodesDictionary,
				Edges = edgesDictionary,
				AmbientParameters = ambientParameters
			};
				
		}
		#endregion

	}
}

