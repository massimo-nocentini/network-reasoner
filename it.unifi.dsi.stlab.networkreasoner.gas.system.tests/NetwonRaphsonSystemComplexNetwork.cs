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
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemComplexNetwork
	{
		GasNetwork aGasNetwork{ get; set; }

		const string IdentifierNodeA = "nodeA";
		const string IdentifierNodeB = "nodeB";
		const string IdentifierNodeC = "nodeC";
		const string IdentifierNodeD = "nodeD";
		const string IdentifierNodeE = "nodeE";
		const string IdentifierNodeF = "nodeF";
		const string IdentifierNodeG = "nodeG";
		const string IdentifierNodeH = "nodeH";
		const string IdentifierNodeI = "nodeI";
		const String IdentifierEdgeAB = "eAB";
		const String IdentifierEdgeBC = "eBC";
		const String IdentifierEdgeCD = "eCD";
		const String IdentifierEdgeAE = "eAE";
		const String IdentifierEdgeAF = "eAF";
		const String IdentifierEdgeBF = "eBF";
		const String IdentifierEdgeBG = "eBG";
		const String IdentifierEdgeCG = "eCG";
		const String IdentifierEdgeEF = "eEF";
		const String IdentifierEdgeEH = "eEH";
		const String IdentifierEdgeGI = "eGI";

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
				IdentifierEdgeAB,
				1000,
				55,
				100);

			GasEdgeAbstract edgeBC = makeEdge (
				nodeBpassive,
				nodeCpassive,
				IdentifierEdgeBC,
				2000,
				55,
				150);

			GasEdgeAbstract edgeCD = makeEdge (
				nodeCpassive,
				nodeDwithLoadGadget,
				IdentifierEdgeCD,
				500,
				55,
				80);

			GasEdgeAbstract edgeAE = makeEdge (
				nodeAwithSupplyGadget,
				nodeEpassive,
				IdentifierEdgeAE,
				1000,
				55,
				100);

			GasEdgeAbstract edgeAF = makeEdge (
				nodeAwithSupplyGadget,
				nodeFwithLoadGadger,
				IdentifierEdgeAF,
				1500,
				55,
				100);

			GasEdgeAbstract edgeBF = makeEdge (
				nodeBpassive,
				nodeFwithLoadGadger,
				IdentifierEdgeBF,
				1000,
				55,
				100);

			GasEdgeAbstract edgeBG = makeEdge (
				nodeBpassive,
				nodeGwithSupplyGadget,
				IdentifierEdgeBG,
				1500,
				55,
				100);

			GasEdgeAbstract edgeCG = makeEdge (
				nodeCpassive,
				nodeGwithSupplyGadget,
				IdentifierEdgeCG,
				500,
				55,
				100);

			GasEdgeAbstract edgeEF = makeEdge (
				nodeEpassive,
				nodeFwithLoadGadger,
				IdentifierEdgeEF,
				1000,
				55,
				50);

			GasEdgeAbstract edgeEH = makeEdge (
				nodeEpassive,
				nodeHwithLoadGadget,
				IdentifierEdgeEH,
				500,
				55,
				80);

			GasEdgeAbstract edgeGI = makeEdge (
				nodeGwithSupplyGadget,
				nodeIwithLoadGadger,
				IdentifierEdgeGI,
				500,
				55,
				80);

			this.aGasNetwork.Nodes.Add (IdentifierNodeA, nodeAwithSupplyGadget);
			this.aGasNetwork.Nodes.Add (IdentifierNodeB, nodeBpassive);
			this.aGasNetwork.Nodes.Add (IdentifierNodeC, nodeCpassive);
			this.aGasNetwork.Nodes.Add (IdentifierNodeD, nodeDwithLoadGadget);
			this.aGasNetwork.Nodes.Add (IdentifierNodeE, nodeEpassive);
			this.aGasNetwork.Nodes.Add (IdentifierNodeF, nodeFwithLoadGadger);
			this.aGasNetwork.Nodes.Add (IdentifierNodeG, nodeGwithSupplyGadget);
			this.aGasNetwork.Nodes.Add (IdentifierNodeH, nodeHwithLoadGadget);
			this.aGasNetwork.Nodes.Add (IdentifierNodeI, nodeIwithLoadGadger);


			this.aGasNetwork.Edges.Add (IdentifierEdgeAB, edgeAB);
			this.aGasNetwork.Edges.Add (IdentifierEdgeAE, edgeAE);
			this.aGasNetwork.Edges.Add (IdentifierEdgeAF, edgeAF);
			this.aGasNetwork.Edges.Add (IdentifierEdgeBC, edgeBC);
			this.aGasNetwork.Edges.Add (IdentifierEdgeBF, edgeBF);
			this.aGasNetwork.Edges.Add (IdentifierEdgeBG, edgeBG);
			this.aGasNetwork.Edges.Add (IdentifierEdgeCD, edgeCD);
			this.aGasNetwork.Edges.Add (IdentifierEdgeCG, edgeCG);
			this.aGasNetwork.Edges.Add (IdentifierEdgeEF, edgeEF);
			this.aGasNetwork.Edges.Add (IdentifierEdgeEH, edgeEH);
			this.aGasNetwork.Edges.Add (IdentifierEdgeGI, edgeGI);
		}

		GasNodeAbstract makeNodeA ()
		{
			GasNodeAbstract supplyNode = new GasNodeTopological{
				Comment = "node A with supply gadget",
				Height = 0,
				Identifier = IdentifierNodeA
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
				Identifier = IdentifierNodeB
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
				Identifier = IdentifierNodeC
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
				Identifier = IdentifierNodeD
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
				Identifier = IdentifierNodeE
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
				Identifier = IdentifierNodeF
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
				Identifier = IdentifierNodeG
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
				Identifier = IdentifierNodeH
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
				Identifier = IdentifierNodeI
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
			string identifier,
			long length,
			double roughness,
			double diameter)
		{
			GasEdgeAbstract anEdgeAB = new GasEdgeTopological{
				StartNode = aStartNode,
				EndNode = anEndNode,
				Identifier = identifier
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
		public void do_mutation_via_repetition ()
		{
			RunnableSystem runnableSystem = new ComplexNetworkRunnableSystem {
				LogConfigFileInfo = new FileInfo (
					"log4net-configurations/for-complex-network.xml"),
				Precision = 75e-6,
				UnknownInitialization = new UnknownInitializationSimplyRandomized ()
			};

			runnableSystem = new RunnableSystemWithDecorationComputeCompletedHandler{
				DecoredRunnableSystem = runnableSystem,
				OnComputeCompletedHandler = assertionsOnFinalState
			};

			runnableSystem.compute ("complex network system",
			                       this.aGasNetwork.Nodes,
			                       this.aGasNetwork.Edges,
			                       this.valid_initial_ambient_parameters ());
		}

		void assertionsOnFinalState (String systemName, FluidDynamicSystemStateAbstract aSystemState)
		{
			Assert.That (aSystemState, Is.InstanceOf (typeof(FluidDynamicSystemStateMathematicallySolved)));

			OneStepMutationResults results = 
				(aSystemState as FluidDynamicSystemStateMathematicallySolved).MutationResult;
			
			assertsOnNodesPressures (results);

			assertsOnEdgesFlows (results);
		}

		void assertsOnNodesPressures (OneStepMutationResults results)
		{
			Vector<NodeForNetwonRaphsonSystem> relativeUnknowns = 
				results.makeUnknownsDimensional ().WrappedObject;

			var nodeA = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeA);
			var nodeB = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeB);
			var nodeC = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeC);
			var nodeD = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeD);
			var nodeE = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeE);
			var nodeF = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeF);
			var nodeG = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeG);
			var nodeH = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeH);
			var nodeI = results.StartingUnsolvedState.findNodeByIdentifier (IdentifierNodeI);

			Assert.That (relativeUnknowns.valueAt (nodeA), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeB), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeC), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeD), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeE), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeF), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeG), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeH), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (relativeUnknowns.valueAt (nodeI), Is.EqualTo (32.34).Within (1e-5));
		}

		void assertsOnEdgesFlows (OneStepMutationResults results)
		{
			var edgeAB = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeAB);
			var edgeAE = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeAE);
			var edgeAF = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeAF);
			var edgeBC = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeBC);
			var edgeBF = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeBF);
			var edgeBG = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeBG);
			var edgeCD = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeCD);
			var edgeCG = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeCG);
			var edgeEF = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeEF);
			var edgeEH = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeEH);
			var edgeGI = results.StartingUnsolvedState.findEdgeByIdentifier (IdentifierEdgeGI);

			Assert.That (results.Qvector.valueAt (edgeAB), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeAE), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeAF), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeBC), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeBF), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeBG), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeCD), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeCG), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeEF), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeEH), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (results.Qvector.valueAt (edgeGI), Is.EqualTo (32.34).Within (1e-5));
		}
	}
}

