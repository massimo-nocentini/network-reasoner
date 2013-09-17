using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using log4net;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.IO;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemRingNetwork
	{
		GasNetwork aGasNetwork{ get; set; }

		[SetUp()]
		public void SetupNetwork ()
		{
			this.aGasNetwork = new GasNetwork ();

			// Is it right to assume the GRF node to be the
			// node 0 in the Fabio's draw?
			GasNodeAbstract GRF = this.makeGRFnode ();

//			GasNodeAbstract node0 = this.makeNode0 ();
			GasNodeAbstract node1 = this.makeNode1 ();
			GasNodeAbstract node2 = this.makeNode2 ();
			GasNodeAbstract node3 = this.makeNode3 ();
			GasNodeAbstract node4 = this.makeNode4 ();
			GasNodeAbstract nodeA = this.makeNodeA ();
			GasNodeAbstract nodeB = this.makeNodeB ();
			GasNodeAbstract nodeC = this.makeNodeC ();
			GasNodeAbstract nodeD = this.makeNodeD ();

			this.aGasNetwork.Nodes.Add ("GRF", GRF);
//			this.aGasNetwork.Nodes.Add ("node0", node0);
			this.aGasNetwork.Nodes.Add ("node1", node1);
			this.aGasNetwork.Nodes.Add ("node2", node2);
			this.aGasNetwork.Nodes.Add ("node3", node3);
			this.aGasNetwork.Nodes.Add ("node4", node4);
			this.aGasNetwork.Nodes.Add ("nodeA", nodeA);
			this.aGasNetwork.Nodes.Add ("nodeB", nodeB);
			this.aGasNetwork.Nodes.Add ("nodeC", nodeC);
			this.aGasNetwork.Nodes.Add ("nodeD", nodeD);

			GasEdgeAbstract edge4 = this.makeEdge4 (GRF, node1);
			GasEdgeAbstract edge5 = this.makeEdge5 (GRF, node4);
			GasEdgeAbstract edge6 = this.makeEdge6 (node1, nodeA);
			GasEdgeAbstract edge7 = this.makeEdge7 (node1, node2);
			GasEdgeAbstract edge8 = this.makeEdge8 (node2, nodeB);
			GasEdgeAbstract edge9 = this.makeEdge9 (node2, node3);
			GasEdgeAbstract edge10 = this.makeEdge10 (node4, nodeC);
			GasEdgeAbstract edge11 = this.makeEdge11 (node3, node4);
			GasEdgeAbstract edge12 = this.makeEdge12 (node3, nodeD);

			this.aGasNetwork.Edges.Add ("edge4", edge4);
			this.aGasNetwork.Edges.Add ("edge5", edge5);
			this.aGasNetwork.Edges.Add ("edge6", edge6);
			this.aGasNetwork.Edges.Add ("edge7", edge7);
			this.aGasNetwork.Edges.Add ("edge8", edge8);
			this.aGasNetwork.Edges.Add ("edge9", edge9);
			this.aGasNetwork.Edges.Add ("edge10", edge10);
			this.aGasNetwork.Edges.Add ("edge11", edge11);
			this.aGasNetwork.Edges.Add ("edge12", edge12);

		}

		GasNodeAbstract makeGRFnode ()
		{
			GasNodeAbstract GRF = new GasNodeTopological{
				Identifier = "GRF",
				Height = 1000
			};

			GasNodeGadgetSupply gadget = new GasNodeGadgetSupply{
				MaxQ= 1000,
				MinQ=100,
				SetupPressure = 700
			};

			return new GasNodeWithGadget{ 
				Equipped = GRF,
				Gadget = gadget
			};
		}

//		GasNodeAbstract makeNode0 ()
//		{
//			GasNodeAbstract node = new GasNodeTopological{
//				Identifier = "N0",
//				Height = 400
//			};
//
//			return node;
//		}

		GasNodeAbstract makeNode1 ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "N1",
				Height = 400
			};

			return node;
		}

		GasNodeAbstract makeNode2 ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "N2",
				Height = 400
			};

			return node;
		}

		GasNodeAbstract makeNode3 ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "N3",
				Height = 400
			};

			return node;
		}

		GasNodeAbstract makeNode4 ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "N4",
				Height = 400
			};

			return node;
		}

		GasNodeAbstract makeNodeA ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "NA",
				Height = 1000
			};

			GasNodeGadgetLoad gadget = new GasNodeGadgetLoad{
				Load = 200
			};

			return new GasNodeWithGadget{ 
				Equipped = node,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeB ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "NB",
				Height = 1000
			};

			GasNodeGadgetLoad gadget = new GasNodeGadgetLoad{
				Load = 200
			};

			return new GasNodeWithGadget{ 
				Equipped = node,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeC ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "NC",
				Height = 1000
			};

			GasNodeGadgetLoad gadget = new GasNodeGadgetLoad{
				Load = 150
			};

			return new GasNodeWithGadget{ 
				Equipped = node,
				Gadget = gadget
			};
		}

		GasNodeAbstract makeNodeD ()
		{
			GasNodeAbstract node = new GasNodeTopological{
				Identifier = "ND",
				Height = 1000
			};

			GasNodeGadgetLoad gadget = new GasNodeGadgetLoad{
				Load = 200
			};

			return new GasNodeWithGadget{ 
				Equipped = node,
				Gadget = gadget
			};
		}

		GasEdgeAbstract makeEdge4 (GasNodeAbstract gRF, GasNodeAbstract node1)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = gRF,
				EndNode = node1
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=120,
				Length=5000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge5 (GasNodeAbstract gRF, GasNodeAbstract node4)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = gRF,
				EndNode = node4
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=120,
				Length=5000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge6 (GasNodeAbstract node1, GasNodeAbstract nodeA)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node1,
				EndNode = nodeA
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=200,
				Length=3000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge7 (GasNodeAbstract node1, GasNodeAbstract node2)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node1,
				EndNode = node2
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=3000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge8 (GasNodeAbstract node2, GasNodeAbstract nodeB)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node2,
				EndNode = nodeB
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=5000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge9 (GasNodeAbstract node2, GasNodeAbstract node3)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node2,
				EndNode = node3
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=2000,
				MaxSpeed=300,
				Roughness=100
			};

			GasEdgeGadget gadget = new GasEdgeGadgetSwitchOff ();

			return new GasEdgeWithGadget{
				Equipped = edgePhysical,
				Gadget = gadget
			};
		}

		GasEdgeAbstract makeEdge10 (GasNodeAbstract node4, GasNodeAbstract nodeC)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node4,
				EndNode = nodeC
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=3000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge11 (GasNodeAbstract node3, GasNodeAbstract node4)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node3,
				EndNode = node4
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=5000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
		}

		GasEdgeAbstract makeEdge12 (GasNodeAbstract node3, GasNodeAbstract nodeD)
		{
			GasEdgeAbstract edgeTopological = new GasEdgeTopological{
				StartNode = node3,
				EndNode = nodeD
			};

			GasEdgeAbstract edgePhysical = new GasEdgePhysical{
				Described= edgeTopological,
				Diameter=100,
				Length=3000,
				MaxSpeed=300,
				Roughness=100
			};

			return edgePhysical;
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
		public void TestCase ()
		{
			ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));

			XmlConfigurator.Configure (new FileInfo (
				"log4net-configurations/for-ring-network.xml")
			);

			NetwonRaphsonSystem system = new NetwonRaphsonSystem ();
			system.FormulaVisitor = new GasFormulaVisitorExactlyDimensioned{
				AmbientParameters = valid_initial_ambient_parameters()
			};
			system.Log = log;

			system.writeSomeLog ("first interesting test");

			system.initializeWith (this.aGasNetwork);
			var resultsAfterOneMutation = system.mutateWithoutIterationNumber ();
		}
	}
}

