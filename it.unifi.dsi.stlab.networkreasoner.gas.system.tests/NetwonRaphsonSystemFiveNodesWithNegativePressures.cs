using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using log4net;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.IO;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemFiveNodesWithNegativePressures
	{
		class FiveNodesNetworkRunnableSystem : RunnableSystemAbstractComputationalResultHandlerShortTableSummary
		{
			#region RunnableSystem implementation
			public override void compute (
				String systemName,
				Dictionary<string, GasNodeAbstract> nodes, 
				Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));

				XmlConfigurator.Configure (
					new FileInfo ("log4net-configurations/for-five-nodes-network-with-negative-pressures.xml"));

				var formulaVisitor = new GasFormulaVisitorExactlyDimensioned {
					AmbientParameters = ambientParameters
				};

				NetwonRaphsonSystem system = new NetwonRaphsonSystem {
				FormulaVisitor = formulaVisitor,
				EventsListener = new NetwonRaphsonSystemEventsListenerForLoggingSummary{
						Log = log
					}
				};

				var aGasNetwork = new GasNetwork{
					Nodes = nodes,
					Edges = edges,				
					AmbientParameters = ambientParameters
				};

				system.initializeWith (aGasNetwork);

				var untilConditions = new List<UntilConditionAbstract> {
					new UntilConditionAdimensionalRatioPrecisionReached {
						Precision = 1e-8
					}
				};

				var mainComputationResults = system.repeatMutateUntil (untilConditions);

				Dictionary<GasNodeAbstract, GasNodeAbstract> fixedNodesWithLoadGadgetByOriginalNodes = 
				new Dictionary<GasNodeAbstract, GasNodeAbstract> ();

				OneStepMutationResults resultsAfterFixingNodeWithLoadGadgetPressure = 
				system.fixNodesWithLoadGadgetNegativePressure (
					mainComputationResults, 
					untilConditions,
					fixedNodesWithLoadGadgetByOriginalNodes);

				var dimensionalUnknowns = resultsAfterFixingNodeWithLoadGadgetPressure.ComputedBy.
					makeUnknownsDimensional (resultsAfterFixingNodeWithLoadGadgetPressure.Unknowns);

				this.onComputationFinished (systemName, resultsAfterFixingNodeWithLoadGadgetPressure);

			}
			#endregion
		}


		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				"gheonet-textual-networks/five-nodes-network.dat");

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerAllInOneFile ());

			systemRunner.run (new FiveNodesNetworkRunnableSystem ());
		}

		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets_with_splitted_specification ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				"gheonet-textual-networks/five-nodes-network.dat");

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted ("gheonet-textual-networks/five-nodes-network-extension.dat"));

			var fiveNodesNetworkRunnableSystem = new FiveNodesNetworkRunnableSystem ();
			systemRunner.run (fiveNodesNetworkRunnableSystem);

			File.WriteAllText ("gheonet-textual-networks/five-nodes-network-output.dat", 
			                  fiveNodesNetworkRunnableSystem.buildTableSummary ());
			
		}
	}
}

