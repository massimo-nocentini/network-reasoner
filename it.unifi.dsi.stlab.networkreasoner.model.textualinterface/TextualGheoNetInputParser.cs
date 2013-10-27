using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class TextualGheoNetInputParser
	{
		Lazy<List<String>> SpecificationLines{ get; set; }

		public TextualGheoNetInputParser (string filename)
		{
			this.SpecificationLines = new Lazy<List<string>> (
				() => new List<String> (File.ReadLines (filename)));
		}

		public SystemRunnerFromTextualGheoNetInput parse (
			SpecificationAssembler aSpecAssembler)
		{
			List<String> nodesSpecificationLines;

			Dictionary<String, Func<Double, GasNodeAbstract>> delayedNodesConstruction = 
				this.parseNodeDelayedConstruction (
					out nodesSpecificationLines);

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				aSpecAssembler.assemble (
					delayedNodesConstruction, 
					nodesSpecificationLines,
					this
			);

			return systemRunner;
		}

		protected virtual Dictionary<String, Func<Double, GasNodeAbstract>> parseNodeDelayedConstruction (
				out List<String> nodesSpecificationLines)
		{
			Dictionary<String, Func<Double, GasNodeAbstract>> delayConstructedNodes = 
				new Dictionary<string, Func<double, GasNodeAbstract>> ();

			nodesSpecificationLines = SpecificationLines.Value.FindAll (
				line => line.StartsWith ("N"));

			nodesSpecificationLines.ForEach (nodeSpecification => {

				var splittedSpecification = nodeSpecification.Split (' ');

				var nodeIdentifier = splittedSpecification [0];

				Func<Double, GasNodeAbstract> delayedConstruction = 
					aDouble => {

					GasNodeAbstract aNode = new GasNodeTopological{
						Identifier = nodeIdentifier,
						Height = Int64.Parse(splittedSpecification[4])
					};

					if (splittedSpecification [1].Equals ("1")) {
						aNode = new GasNodeWithGadget{
						Equipped = aNode,
						Gadget = new GasNodeGadgetSupply{
							SetupPressure = aDouble
						}
					};
					} else if (splittedSpecification [1].Equals ("0")) {

						// if we set a passive node we forget what the caller of this lambda
						// gives as aDouble because a passive node is characterized by having
						// a zero load.
						var loadGadget = Double.Parse (splittedSpecification [2]) == 0.0 ?
							new GasNodeGadgetLoad{ Load = 0 } : 
							new GasNodeGadgetLoad{ Load = aDouble };

						aNode = new GasNodeWithGadget{
						Equipped = aNode,
						Gadget = loadGadget
					};
					} else {
						throw new ArgumentException (string.Format (
						"The specification for node {0} is not correct: impossible " +
							"to parse neither supply nor load value.", nodeIdentifier)
						);
					}

					return aNode;
				};

				delayConstructedNodes.Add (nodeIdentifier, delayedConstruction);
			}
			);

			return delayConstructedNodes;
		}

		internal virtual Dictionary<string, GasEdgeAbstract> parseEdgesRelating (
			Dictionary<string, GasNodeAbstract> nodes)
		{
			Dictionary<string, GasEdgeAbstract> parsedEdges = 
				new Dictionary<string, GasEdgeAbstract> ();


			var edgesSpecificationLines = SpecificationLines.Value.FindAll (line => line.StartsWith ("R"));

			edgesSpecificationLines.ForEach (edgeSpecification => {

				var splittedSpecification = edgeSpecification.Split (' ');

				var edgeIdentifier = splittedSpecification [0];

				GasEdgeAbstract anEdge = new GasEdgeTopological{
					StartNode = nodes[splittedSpecification[1]],
					EndNode = nodes[splittedSpecification[2]]
				};
			
				anEdge = new GasEdgePhysical{
					Described = anEdge,
					Diameter = Double.Parse(splittedSpecification[3]),
					Length = Double.Parse(splittedSpecification[4]),
					Roughness = Double.Parse(splittedSpecification[5])
				};

				parsedEdges.Add (edgeIdentifier, anEdge);
			}
			);

			return parsedEdges;
		}

		internal virtual AmbientParameters parseAmbientParameters ()
		{
			AmbientParameters result = new AmbientParametersGas ();

			string ambientParametersLine = this.SpecificationLines.Value [1];
			string[] splittedSpecification = ambientParametersLine.Split (' ');

			// parametriCalcoloGas.pressioneAtmosferica
			result.AirPressureInBar = Double.Parse (splittedSpecification [2]) / 1000; 

			// parametriCalcoloGas.temperaturaAria
			result.AirTemperatureInKelvin = Double.Parse (splittedSpecification [5]) + 273.15;
			result.ElementName = "methane";

			// parametriCalcoloGas.temperaturaGas
			result.ElementTemperatureInKelvin = Double.Parse (splittedSpecification [4]) + 273.15;

			// parametriCalcoloGas.pesoMolecolareGas
			result.MolWeight = Double.Parse (splittedSpecification [6]);
			result.RefPressureInBar = 1.01325;
			result.RefTemperatureInKelvin = 288.15; // da interfacciare

			// parametriCalcoloGas.viscositaGas
			result.ViscosityInPascalTimesSecond = Double.Parse (splittedSpecification [9]) * 1e-3;

			return result;
		}



	}
}

