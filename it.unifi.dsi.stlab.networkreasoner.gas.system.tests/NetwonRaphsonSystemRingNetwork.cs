using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using log4net;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.IO;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;

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
				EndNode = node1,
				Identifier = "edge4"
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
				EndNode = node4,
				Identifier = "edge5"
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
				EndNode = nodeA,
				Identifier = "edge6"
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
				EndNode = node2,
				Identifier = "edge7"
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
				EndNode = nodeB,
				Identifier = "edge8"
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
				EndNode = node3,
				Identifier = "edge9"

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
				EndNode = nodeC,
				Identifier = "edge10"
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
				EndNode = node4,
				Identifier = "edge11"
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
				EndNode = nodeD,
				Identifier = "edge12"
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

		class ComplexNetworkRunnableSystem : RunnableSystemCompute
		{
			protected override List<FluidDynamicSystemStateTransition> buildTransitionsSequence (
				FluidDynamicSystemStateTransitionInitialization initializationTransition, 
				FluidDynamicSystemStateTransitionNewtonRaphsonSolve solveTransition, 
				FluidDynamicSystemStateTransitionNegativeLoadsChecker negativeLoadsCheckerTransition)
			{
				return new List<FluidDynamicSystemStateTransition>{
					initializationTransition, solveTransition};
			}
		}

		[Test()]
		public void test_ring_network_using_complete_solving_system ()
		{
			RunnableSystem runnableSystem = new ComplexNetworkRunnableSystem {
				LogConfigFileInfo = new FileInfo (
					"log4net-configurations/for-ring-network.xml"),
				Precision = 75e-6,
				UnknownInitialization = new UnknownInitializationSimplyRandomized ()
			};

			runnableSystem = new RunnableSystemWithDecorationComputeCompletedHandler{
				DecoredRunnableSystem = runnableSystem,
				OnComputeCompletedHandler = checkAssertions
			};

			runnableSystem.compute ("ring network system",
			                       this.aGasNetwork.Nodes,
			                       this.aGasNetwork.Edges,
			                       this.valid_initial_ambient_parameters ());

		}

		void checkAssertions (String systemName, FluidDynamicSystemStateAbstract aSystemState)
		{
			Assert.That (aSystemState, Is.InstanceOf (
				typeof(FluidDynamicSystemStateMathematicallySolved))
			);

			OneStepMutationResults results = 
				(aSystemState as FluidDynamicSystemStateMathematicallySolved).MutationResult;

			
			var dimensionalUnknowns = results.makeUnknownsDimensional ().WrappedObject;

			var node1 = results.StartingUnsolvedState.findNodeByIdentifier ("N1");
			var node2 = results.StartingUnsolvedState.findNodeByIdentifier ("N2");
			var node3 = results.StartingUnsolvedState.findNodeByIdentifier ("N3");
			var node4 = results.StartingUnsolvedState.findNodeByIdentifier ("N4");
			var nodeGRF = results.StartingUnsolvedState.findNodeByIdentifier ("GRF");
			var nodeA = results.StartingUnsolvedState.findNodeByIdentifier ("NA");
			var nodeB = results.StartingUnsolvedState.findNodeByIdentifier ("NB");
			var nodeC = results.StartingUnsolvedState.findNodeByIdentifier ("NC");
			var nodeD = results.StartingUnsolvedState.findNodeByIdentifier ("ND");
			var precision = 1e-1;
			Assert.That (dimensionalUnknowns.valueAt (node1), Is.EqualTo (529.643).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (node2), Is.EqualTo (444.374).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (node3), Is.EqualTo (435.396).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (node4), Is.EqualTo (575.737).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (nodeGRF), Is.EqualTo (699.999).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (nodeA), Is.EqualTo (506.404).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (nodeB), Is.EqualTo (272.014).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (nodeC), Is.EqualTo (505.433).Within (precision));
			Assert.That (dimensionalUnknowns.valueAt (nodeD), Is.EqualTo (327.100).Within (precision));
			
			var edge4 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge4");
			var edge5 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge5");
			var edge6 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge6");
			var edge7 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge7");
			var edge8 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge8");
			var edge9 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge9");
			var edge10 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge10");
			var edge11 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge11");
			var edge12 = results.StartingUnsolvedState.findEdgeByIdentifier ("edge12");

			Assert.That (results.Qvector.valueAt (edge4), Is.EqualTo (400).Within (precision));
			Assert.That (results.Qvector.valueAt (edge5), Is.EqualTo (350).Within (precision));
			Assert.That (results.Qvector.valueAt (edge6), Is.EqualTo (200).Within (precision));
			Assert.That (results.Qvector.valueAt (edge7), Is.EqualTo (200).Within (precision));
			Assert.That (results.Qvector.valueAt (edge8), Is.EqualTo (200).Within (precision));
			Assert.That (results.Qvector.containsKey (edge9), Is.False);
			Assert.That (results.Qvector.valueAt (edge10), Is.EqualTo (150).Within (precision));
			Assert.That (results.Qvector.valueAt (edge11), Is.EqualTo (-200).Within (precision));
			Assert.That (results.Qvector.valueAt (edge12), Is.EqualTo (200).Within (precision));		
		}
	}
}

