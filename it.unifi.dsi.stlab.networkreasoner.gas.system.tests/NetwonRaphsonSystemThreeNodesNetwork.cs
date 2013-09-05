using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using log4net;
using log4net.Config;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemThreeNodesNetwork
	{
		GasNetwork aGasNetwork{ get; set; }

		[SetUp()]
		public void SetupNetwork ()
		{
			this.aGasNetwork = new GasNetwork ();

			GasNodeAbstract nodeAwithSupplyGadget = this.makeNodeA ();
			GasNodeAbstract nodeBwithLoadGadget = this.makeNodeB ();
			GasNodeAbstract nodeCwithLoadGadger = this.makeNodeC ();

			GasEdgeAbstract edgeAB = this.makeEdgeAB (
				nodeAwithSupplyGadget, nodeBwithLoadGadget);

			GasEdgeAbstract edgeCB = this.makeEdgeCB (
				nodeCwithLoadGadger, nodeBwithLoadGadget);

			this.aGasNetwork.Nodes.Add ("nodeA", nodeAwithSupplyGadget);
			this.aGasNetwork.Nodes.Add ("nodeB", nodeBwithLoadGadget);
			this.aGasNetwork.Nodes.Add ("nodeC", nodeCwithLoadGadger);
			this.aGasNetwork.Edges.Add ("edgeAB", edgeAB);
			this.aGasNetwork.Edges.Add ("edgeCB", edgeCB);			
		}

		GasNodeAbstract makeNodeA ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node A with supply gadget",
				Height = 0,
				Identifier = "nA"
			};

			GasNodeGadget supplyGadget = new GasNodeGadgetSupply{
				MaxQ = 198.3,
				MinQ = 10.4,
				SetupPressure = 30
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
				Height = 0,
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
				Height = 0,
				Identifier = "nC"
			};

			GasNodeGadget gadget = new GasNodeGadgetLoad{
				Load = 100.0
			};

			return new GasNodeWithGadget{
				Equipped = supplyNode,
				Gadget = gadget
			};
		}

		GasEdgeAbstract makeEdgeAB (GasNodeAbstract aStartNode, GasNodeAbstract anEndNode)
		{
			GasEdgeAbstract anEdgeAB = new GasEdgeTopological{
				StartNode = aStartNode,
				EndNode = anEndNode
			};

			return new GasEdgePhysical{
				Described = anEdgeAB,
				Length = 500,
				Roughness = 55,
				Diameter = 100,
				MaxSpeed = 10
			};
		}

		GasEdgeAbstract makeEdgeCB (GasNodeAbstract aStartNode, GasNodeAbstract anEndNode)
		{
			GasEdgeAbstract anEdgeCB = new GasEdgeTopological{
				StartNode = aStartNode,
				EndNode = anEndNode
			};

			return new GasEdgePhysical{
				Described = anEdgeCB,
				Length = 500,
				Roughness = 55,
				Diameter = 100,
				MaxSpeed = 10
			};
		}

		public AmbientParameters valid_initial_ambient_parameters ()
		{
			AmbientParameters parameters = new AmbientParameters ();
			parameters.GasName = "methane";
			parameters.MolWeight = 16.0;
			parameters.GasTemperatureInKelvin = 288.15;
			parameters.RefPressureInBar = 1.01325;
			parameters.RefTemperatureInKelvin = 288.15;
			parameters.AirPressureInBar = 1.01325;
			parameters.AirTemperatureInKelvin = 288.15;
			parameters.ViscosityInPascalTimesSecond = .0000108;

			return parameters;

		}

		[Test()]
		public void do_one_mutation_step ()
		{
			ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));

			XmlConfigurator.Configure (new FileInfo (
				"log4net-configurations/for-three-nodes-network.xml")
			);

			NetwonRaphsonSystem system = new NetwonRaphsonSystem ();
			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned {
				AmbientParameters = valid_initial_ambient_parameters ()
			};
			system.FormulaVisitor = formulaVisitor;
			system.Log = log;

			system.writeSomeLog ("first interesting test");

			system.initializeWith (this.aGasNetwork);
			var resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();
			resultsAfterOneMutation = system.mutate ();

			var relativeUnknowns = system.denormalizeUnknowns ();


		}
	}
}

