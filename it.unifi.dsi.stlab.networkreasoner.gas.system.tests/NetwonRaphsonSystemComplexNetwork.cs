using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using log4net;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using System.Collections.Generic;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemComplexNetwork
	{
		GasNetwork aGasNetwork{ get; set; }

		[SetUp()]
		public void SetupNetwork ()
		{
			this.aGasNetwork = new GasNetwork ();

			GasNodeAbstract nodeAwithSupplyGadget = this.makeNodeA ();
			GasNodeAbstract nodeBpassive = this.makeNodeB ();
			GasNodeAbstract nodeCpassive = this.makeNodeC ();
			GasNodeAbstract nodeDwithLoadGadget = this.makeNodeD ();
			GasNodeAbstract nodeEpassive = this.makeNodeE ();
			GasNodeAbstract nodeFwithLoadGadger = this.makeNodeF ();
			GasNodeAbstract nodeGwithSupplyGadget = this.makeNodeG ();
			GasNodeAbstract nodeHwithLoadGadget = this.makeNodeH ();
			GasNodeAbstract nodeIwithLoadGadger = this.makeNodeI ();

			GasEdgeAbstract edgeAB = makeEdge (
				nodeAwithSupplyGadget,
				nodeBpassive,
				1000,
				55,
				100);

			GasEdgeAbstract edgeBC = makeEdge (
				nodeBpassive,
				nodeCpassive,
				2000,
				55,
				150);

			GasEdgeAbstract edgeCD = makeEdge (
				nodeCpassive,
				nodeDwithLoadGadget,
				500,
				55,
				80);

			GasEdgeAbstract edgeAE = makeEdge (
				nodeAwithSupplyGadget,
				nodeEpassive,
				1000,
				55,
				100);

			GasEdgeAbstract edgeAF = makeEdge (
				nodeAwithSupplyGadget,
				nodeFwithLoadGadger,
				1500,
				55,
				100);

			GasEdgeAbstract edgeBF = makeEdge (
				nodeBpassive,
				nodeFwithLoadGadger,
				1000,
				55,
				100);

			GasEdgeAbstract edgeBG = makeEdge (
				nodeBpassive,
				nodeGwithSupplyGadget,
				1500,
				55,
				100);

			GasEdgeAbstract edgeCG = makeEdge (
				nodeCpassive,
				nodeGwithSupplyGadget,
				500,
				55,
				100);

			GasEdgeAbstract edgeEF = makeEdge (
				nodeEpassive,
				nodeFwithLoadGadger,
				1000,
				55,
				50);

			GasEdgeAbstract edgeEH = makeEdge (
				nodeEpassive,
				nodeHwithLoadGadget,
				500,
				55,
				80);

			GasEdgeAbstract edgeGI = makeEdge (
				nodeGwithSupplyGadget,
				nodeIwithLoadGadger,
				500,
				55,
				80);

			this.aGasNetwork.Nodes.Add ("nodeA", nodeAwithSupplyGadget);
			this.aGasNetwork.Nodes.Add ("nodeB", nodeBpassive);
			this.aGasNetwork.Nodes.Add ("nodeC", nodeCpassive);
			this.aGasNetwork.Nodes.Add ("nodeD", nodeDwithLoadGadget);
			this.aGasNetwork.Nodes.Add ("nodeE", nodeEpassive);
			this.aGasNetwork.Nodes.Add ("nodeF", nodeFwithLoadGadger);
			this.aGasNetwork.Nodes.Add ("nodeG", nodeGwithSupplyGadget);
			this.aGasNetwork.Nodes.Add ("nodeH", nodeHwithLoadGadget);
			this.aGasNetwork.Nodes.Add ("nodeI", nodeIwithLoadGadger);


			this.aGasNetwork.Edges.Add ("edgeAB", edgeAB);
			this.aGasNetwork.Edges.Add ("edgeAE", edgeAE);
			this.aGasNetwork.Edges.Add ("edgeAF", edgeAF);
			this.aGasNetwork.Edges.Add ("edgeBC", edgeBC);
			this.aGasNetwork.Edges.Add ("edgeBF", edgeBF);
			this.aGasNetwork.Edges.Add ("edgeBG", edgeBG);
			this.aGasNetwork.Edges.Add ("edgeCD", edgeCD);
			this.aGasNetwork.Edges.Add ("edgeCG", edgeCG);
			this.aGasNetwork.Edges.Add ("edgeEF", edgeEF);
			this.aGasNetwork.Edges.Add ("edgeEH", edgeEH);
			this.aGasNetwork.Edges.Add ("edgeGI", edgeGI);
		}

		GasNodeAbstract makeNodeA ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node A with supply gadget",
				Height = 0,
				Identifier = "nA"
			};

			GasNodeGadget supplyGadget = new GasNodeGadgetSupply{
				SetupPressure = 5000
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = supplyGadget
			};
		}

		GasNodeAbstract makeNodeB ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node B with load gadget",
				Height = 50,
				Identifier = "nB"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 0.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeC ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node C with supply gadget",
				Height = 500,
				Identifier = "nC"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 0.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeD ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node D with supply gadget",
				Height = 500,
				Identifier = "nD"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 100.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeE ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node E with supply gadget",
				Height = 0,
				Identifier = "nE"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 0.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeF ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node F with supply gadget",
				Height = 0,
				Identifier = "nF"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 200.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeG ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node G with supply gadget",
				Height = 200,
				Identifier = "nG"
			};

			GasNodeGadget gadget = new GasNodeGadgetSupply{
			 	SetupPressure = 5000
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeH ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node H with supply gadget",
				Height = 100,
				Identifier = "nH"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 100.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeI ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node I with supply gadget",
				Height = 200,
				Identifier = "nI"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 150.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasEdgeAbstract makeEdge (
			GasNodeAbstract aStartNode, 
			GasNodeAbstract anEndNode,
			long length,
			double roughness,
			double diameter)
		{
			GasEdgeAbstract anEdgeAB = new GasEdgeTopological{
				StartNode = aStartNode,
				EndNode = anEndNode
			};

			return new GasEdgePhysical{
				Described = anEdgeAB,
				Length = length,
				Roughness = roughness,
				Diameter = diameter
			};
		}

		public AmbientParameters valid_initial_ambient_parameters ()
		{
			AmbientParameters parameters = new AmbientParametersGas ();
			parameters.ElementName = "methane";
			parameters.MolWeight = 16.0;
			parameters.ElementTemperatureInKelvin = 288.15;
			parameters.RefPressureInBar = 1.01325;
			parameters.RefTemperatureInKelvin = 288.15;
			parameters.AirPressureInBar = 1.01325;
			parameters.AirTemperatureInKelvin = 288.15;
			parameters.ViscosityInPascalTimesSecond = .0000108;

			return parameters;


		}

		[Test()]
		public void do_mutation_via_repetition ()
		{
			ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));

			XmlConfigurator.Configure (new FileInfo (
				"log4net-configurations/for-complex-network.xml")
			);

			var ambientParameters = valid_initial_ambient_parameters ();
			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned {
				AmbientParameters = ambientParameters
			};

			NetwonRaphsonSystemInterface system = new NetwonRaphsonSystem {
				FormulaVisitor = formulaVisitor,
				EventsListener = new NetwonRaphsonSystemEventsListenerForLoggingSummary{
					Log = log
				}
			};

			this.aGasNetwork.AmbientParameters = ambientParameters;
			system.initializeWith (this.aGasNetwork);

			var resultsAfterOneMutation = system.repeatMutateUntil (
				new List<UntilConditionAbstract>{
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = 75e-6
				}
			}
			);

			var relativeUnknowns = system.denormalizeUnknowns ();
		}
	}
}

