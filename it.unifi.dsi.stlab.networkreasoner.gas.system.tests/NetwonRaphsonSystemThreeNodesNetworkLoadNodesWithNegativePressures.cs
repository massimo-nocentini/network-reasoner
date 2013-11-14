using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using log4net;
using log4net.Config;
using System.IO;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.utilities.object_with_substitution;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemThreeNodesNetworkLoadNodesWithNegativePressures
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
				Load = 180.0
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
				EndNode = anEndNode,
				Identifier = "edgeAB"
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
				EndNode = anEndNode,
				Identifier = "edgeCB"
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
		public void do_mutation_via_repetition_checking_pressure_correctness_after_main_computation ()
		{
			ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));

			XmlConfigurator.Configure (new FileInfo (
				"log4net-configurations/for-three-nodes-network.xml")
			);

			var ambientParameters = valid_initial_ambient_parameters ();
			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned {
				AmbientParameters = ambientParameters
			};

			NetwonRaphsonSystem system = new NetwonRaphsonSystem {
				FormulaVisitor = formulaVisitor,
				EventsListener = new NetwonRaphsonSystemEventsListenerForLoggingSummary{
					Log = log
				}
			};

			this.aGasNetwork.AmbientParameters = ambientParameters;
			system.initializeWith (this.aGasNetwork);

			var untilConditions = new List<UntilConditionAbstract> {
				new UntilConditionAdimensionalRatioPrecisionReached {
					Precision = 1e-4
				}
			};

			var mainComputationResults = system.repeatMutateUntil (untilConditions);

			var nodesSubstitutions = 
				new List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> ();
			var edgesSubstitutions = 
				new List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> ();

			OneStepMutationResults resultsAfterFixingNodeWithLoadGadgetPressure = 
				system.fixNodesWithLoadGadgetNegativePressure (
					mainComputationResults, 
					untilConditions,
					nodesSubstitutions,
					edgesSubstitutions);

			var dimensionalUnknownsWrapper = resultsAfterFixingNodeWithLoadGadgetPressure.ComputedBy.
				makeUnknownsDimensional (
					resultsAfterFixingNodeWithLoadGadgetPressure.Unknowns);

			var dimensionalUnknowns = dimensionalUnknownsWrapper.WrappedObject;

			var nodeA = resultsAfterFixingNodeWithLoadGadgetPressure.findNodeByIdentifier ("nA");
			var nodeB = resultsAfterFixingNodeWithLoadGadgetPressure.findNodeByIdentifier ("nB");
			var nodeC = resultsAfterFixingNodeWithLoadGadgetPressure.findNodeByIdentifier ("nC");
			Assert.That (dimensionalUnknowns.valueAt (nodeA), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (dimensionalUnknowns.valueAt (nodeB), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (dimensionalUnknowns.valueAt (nodeC), Is.EqualTo (32.34).Within (1e-5));

			var edgeAB = resultsAfterFixingNodeWithLoadGadgetPressure.findEdgeByIdentifier ("edgeAB");
			var edgeCB = resultsAfterFixingNodeWithLoadGadgetPressure.findEdgeByIdentifier ("edgeCB");
			Assert.That (resultsAfterFixingNodeWithLoadGadgetPressure.Qvector.valueAt (edgeAB), Is.EqualTo (32.34).Within (1e-5));
			Assert.That (resultsAfterFixingNodeWithLoadGadgetPressure.Qvector.valueAt (edgeCB), Is.EqualTo (32.34).Within (1e-5));

		}
	}
}

