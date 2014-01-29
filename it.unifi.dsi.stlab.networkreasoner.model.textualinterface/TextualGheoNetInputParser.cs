using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;
using it.unifi.dsi.stlab.utilities.value_holders;
using System.Globalization;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class TextualGheoNetInputParser
	{
		Lazy<List<String>> SpecificationLines{ get; set; }

		public TextualGheoNetInputParser (List<String> lines)
		{
			this.SpecificationLines = new Lazy<List<string>> (
				() => {
				var result = new List<String> ();
				lines.ForEach (line => result.Add (line.Trim ()));
				return result;
			}
			);
		}

		public TextualGheoNetInputParser (FileInfo file)
		{
			this.SpecificationLines = new Lazy<List<string>> (
				() => {
				var lines = new List<String> (File.ReadLines (file.FullName));
				var result = new List<String> ();
				lines.ForEach (line => result.Add (line.Trim ()));
				return result;
			}
			);
		}

		public SystemRunnerFromTextualGheoNetInput parse (
			SpecificationAssembler aSpecAssembler)
		{
			List<NodeSpecificationLine> nodesSpecificationLines;

			Dictionary<String, Func<ValueHolder<Double>, GasNodeAbstract>> delayedNodesConstruction = 
				this.parseNodeDelayedConstruction (
					out nodesSpecificationLines);

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				aSpecAssembler.assemble (
					delayedNodesConstruction, 
					nodesSpecificationLines,
					this);

			return systemRunner;
		}

		public virtual List<string> fetchRegion (string regionIdentifier, TableHeaderParser tableHeaderParser)
		{
			int startNodesRegionIndex = SpecificationLines.Value.FindIndex (
				line => line.StartsWith (string.Format ("* {0}", regionIdentifier)));

			int endNodesRegionIndex = SpecificationLines.Value.FindIndex (
				startNodesRegionIndex + 1,
				line => line.StartsWith ("*"));

			var region = SpecificationLines.Value.GetRange (
				startNodesRegionIndex + 1, endNodesRegionIndex - startNodesRegionIndex - 1);

			region = tableHeaderParser.parse (region);

			region.RemoveAll (aLine => string.IsNullOrEmpty (aLine.Replace ("\t", "")));

			return region;
		}

		protected virtual Dictionary<String, Func<ValueHolder<Double>, GasNodeAbstract>> 
			parseNodeDelayedConstruction (
				out List<NodeSpecificationLine> nodesSpecificationLinesOut)
		{
			Dictionary<String, Func<ValueHolder<Double>, GasNodeAbstract>> delayConstructedNodes = 
				new Dictionary<string, Func<ValueHolder<Double>, GasNodeAbstract>> ();

			var nodesSpecificationLines = new List<NodeSpecificationLine> ();

			var rawNodesSpecificationLines = fetchRegion ("nodes", new TableHeaderParserIgnoreHeader ());

			rawNodesSpecificationLines.ForEach (nodeSpecification => {

				var splittedSpecification = splitOrgRow (nodeSpecification);

				NodeSpecificationLine semanticLine = new NodeSpecificationLine ();

				semanticLine.Identifier = splittedSpecification [0];
				semanticLine.Height = Int64.Parse (splittedSpecification [4]);
				semanticLine.SplittedSpecification = splittedSpecification;
				semanticLine.Type = splittedSpecification [1].Equals ("1") ? 
					NodeType.WithSupplyGadget : NodeType.WithLoadGadget;

				Func<ValueHolder<Double>, GasNodeAbstract> delayedConstruction = 
					aValueHolder => {

					GasNodeAbstract aNode = new GasNodeTopological{
						Identifier = semanticLine.Identifier,
						Height = semanticLine.Height
					};

					if (semanticLine.Type == NodeType.WithSupplyGadget) {

						aNode = new GasNodeWithGadget{
							Equipped = aNode,
							Gadget = new GasNodeGadgetSupply{
								SetupPressure = aValueHolder.getValue()
							}
						};
					} else if (semanticLine.Type == NodeType.WithLoadGadget) {

						// if we set a passive node we forget what the caller of this lambda
						// gives as aDouble because a passive node is characterized by having
						// a zero load.
						var loadGadget = Double.Parse (splittedSpecification [2]) == 0.0 ?
							new GasNodeGadgetLoad{ Load = 0 } : 
							new GasNodeGadgetLoad{ Load = aValueHolder.getValue() };

						aNode = new GasNodeWithGadget{
							Equipped = aNode,
							Gadget = loadGadget
						};

					} else {
						throw new ArgumentException (string.Format (
						"The specification for node {0} is not correct: impossible " +
							"to parse neither supply nor load value.", semanticLine.Identifier)
						);
					}

					return aNode;
				};

				nodesSpecificationLines.Add (semanticLine);

				delayConstructedNodes.Add (semanticLine.Identifier, delayedConstruction);
			}
			);

			nodesSpecificationLinesOut = nodesSpecificationLines;

			return delayConstructedNodes;
		}

		public virtual String[] splitOrgRow (String orgTableRow)
		{
			return orgTableRow.Replace (" ", string.Empty).Replace ("\t", string.Empty).
				Split (new []{'|'}, StringSplitOptions.RemoveEmptyEntries);
		}

		internal virtual Dictionary<string, GasEdgeAbstract> parseEdgesRelating (
			Dictionary<string, GasNodeAbstract> nodes)
		{
			Dictionary<string, GasEdgeAbstract> parsedEdges = 
				new Dictionary<string, GasEdgeAbstract> ();

			var edgesSpecificationLines = fetchRegion ("edges", new TableHeaderParserIgnoreHeader ());

			edgesSpecificationLines.ForEach (edgeSpecification => {

				var splittedSpecification = splitOrgRow (edgeSpecification);

				var edgeIdentifier = splittedSpecification [0];

				GasEdgeAbstract anEdge = new GasEdgeTopological{
					Identifier = edgeIdentifier,
					StartNode = nodes[splittedSpecification[1]],
					EndNode = nodes[splittedSpecification[2]]
				};
			
				anEdge = new GasEdgePhysical{
					Described = anEdge,
					Diameter = parseDoubleCultureInvariant(splittedSpecification[3]).Value,
					Length = parseDoubleCultureInvariant(splittedSpecification[4]).Value,
					Roughness = parseDoubleCultureInvariant(splittedSpecification[5]).Value
				};

				parsedEdges.Add (edgeIdentifier, anEdge);
			}
			);

			return parsedEdges;
		}

		internal virtual AmbientParameters parseAmbientParameters ()
		{
			AmbientParameters result = new AmbientParametersWater ();
			
			var ambientParametersSpecificationLines = fetchRegion ("ambient parameters", 
			                                                       new TableHeaderParserIgnoreHeader ());
			string ambientParametersLine = ambientParametersSpecificationLines [0];
			string[] splittedSpecification = splitOrgRow (ambientParametersLine);

			// parametriCalcoloGas.pressioneAtmosferica
			result.AirPressureInBar = parseDoubleCultureInvariant (
				splittedSpecification [2]).Value * 1e-3; 

			// parametriCalcoloGas.temperaturaAria
			result.AirTemperatureInKelvin = parseDoubleCultureInvariant (
				splittedSpecification [5]).Value + 273.15;

			result.ElementName = "methane";

			// parametriCalcoloGas.temperaturaGas
			result.ElementTemperatureInKelvin = parseDoubleCultureInvariant (
				splittedSpecification [4]).Value + 273.15;

			// parametriCalcoloGas.pesoMolecolareGas
			result.MolWeight = parseDoubleCultureInvariant (
				splittedSpecification [6]).Value;

			result.RefPressureInBar = 1.01325;
			result.RefTemperatureInKelvin = 288.15; // da interfacciare

			// parametriCalcoloGas.viscositaGas
			result.ViscosityInPascalTimesSecond = parseDoubleCultureInvariant (
				splittedSpecification [9]).Value * 1e-3;

			return result;
		}

		public virtual Nullable<double> parseDoubleCultureInvariant (
			String doubleAsString)
		{
			Nullable<Double> result = null;
			double value;
			if (Double.TryParse (doubleAsString, 
			                     System.Globalization.NumberStyles.Any, 
			                     CultureInfo.InvariantCulture, 
			                     out value)) {
				result = value;
			}

			return result;
		}

		public virtual bool existsLineSuchThat (Predicate<String> predicate)
		{
			return SpecificationLines.Value.Exists (predicate);
		}
	}
}

