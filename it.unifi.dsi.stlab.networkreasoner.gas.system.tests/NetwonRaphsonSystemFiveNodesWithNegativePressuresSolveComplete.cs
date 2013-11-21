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
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemFiveNodesWithNegativePressuresSolveComplete
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
				
				var aGasNetwork = new GasNetwork{
					Nodes = nodes,
					Edges = edges,				
					AmbientParameters = ambientParameters
				};

				ILog log = LogManager.GetLogger (typeof(NewtonRaphsonSystem));
			
				var translatorMaker = new dimensional_objects.DimensionalDelegates ();

				XmlConfigurator.Configure (new FileInfo (
				"log4net-configurations/for-five-nodes-network-with-negative-pressures.xml")
				);

				var formulaVisitor = new GasFormulaVisitorExactlyDimensioned ();
				formulaVisitor.AmbientParameters = ambientParameters;

				var eventListener = new NetwonRaphsonSystemEventsListenerForLoggingSummary ();
				eventListener.Log = log;

				var initializationTransition = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
				initializationTransition.EventsListener = eventListener;
				initializationTransition.Network = aGasNetwork;
				initializationTransition.UnknownInitialization = 
					new UnknownInitializationSimplyRandomized ();
				initializationTransition.FromDimensionalToAdimensionalTranslator = 
				translatorMaker.throwExceptionIfThisTranslatorIsCalled<double> (
				"dimensional -> adimensional translation requested when it isn't required.");

				var solveTransition = new FluidDynamicSystemStateTransitionNewtonRaphsonSolveRaiseEventsDecorator ();
				solveTransition.EventsListener = eventListener;
				solveTransition.FormulaVisitor = formulaVisitor;
				solveTransition.FromDimensionalToAdimensionalTranslator = 
				translatorMaker.throwExceptionIfThisTranslatorIsCalled<Vector<NodeForNetwonRaphsonSystem>> (
				"dimensional -> adimensional translation requested when it isn't required.");
				solveTransition.UntilConditions = new List<UntilConditionAbstract> {
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = 1e-4
				}};
			
				var negativeLoadsCheckerTransition = new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
				negativeLoadsCheckerTransition.EventsListener = eventListener;
				negativeLoadsCheckerTransition.FormulaVisitor = formulaVisitor;

				var system = new FluidDynamicSystemStateTransitionCombinator ();
				var finalState = system.applySequenceOnBareState (new List<FluidDynamicSystemStateTransition>{
				initializationTransition, solveTransition, negativeLoadsCheckerTransition}
				) as FluidDynamicSystemStateNegativeLoadsCorrected;

				var originalDomainReverterVisitor = new FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain ();
				finalState.accept (originalDomainReverterVisitor);

				Dictionary<GasNodeAbstract, double> pressuresByNodes = originalDomainReverterVisitor.PressuresByNodes;
				Dictionary<GasEdgeAbstract, double> flowsByEdges = originalDomainReverterVisitor.FlowsByEdges;

				nodes.ForEach ((nodeKey, originalNode) => {
					Assert.That (pressuresByNodes.ContainsKey (originalNode), Is.True);
				}
				);

				edges.ForEach ((edgeKey, originalEdge) => {
					Assert.That (flowsByEdges.ContainsKey (originalEdge), Is.True);
				}
				);

				this.onComputationFinished (systemName, 
				                            finalState.FluidDynamicSystemStateMathematicallySolved.MutationResult);

			}
			#endregion


		
		}


		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets_with_splitted_specification ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("gheonet-textual-networks/five-nodes-network.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted (
					new FileInfo ("gheonet-textual-networks/five-nodes-network-extension.dat"))
			);

			var fiveNodesNetworkRunnableSystem = new FiveNodesNetworkRunnableSystem ();
			systemRunner.run (fiveNodesNetworkRunnableSystem);

			File.WriteAllText ("gheonet-textual-networks/five-nodes-network-output.dat", 
			                  fiveNodesNetworkRunnableSystem.buildSummaryContent ());
			
		}

	}
}

