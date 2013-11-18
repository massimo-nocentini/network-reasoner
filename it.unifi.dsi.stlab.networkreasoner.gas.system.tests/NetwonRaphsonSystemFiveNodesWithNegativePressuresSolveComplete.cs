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

				ILog log = LogManager.GetLogger (typeof(NetwonRaphsonSystem));
			
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


				Dictionary<GasNodeAbstract, double> nodesPressures;
				Dictionary<GasEdgeAbstract, double> edgesFlows;
				visitNegativeLoadsCheckedSystemState (finalState, 
				                            out nodesPressures, 
				                            out edgesFlows);

				nodes.ForEach ((nodeKey, originalNode) => {
					Assert.That (nodesPressures.ContainsKey (originalNode), Is.True);
				}
				);

				edges.ForEach ((edgeKey, originalEdge) => {
					Assert.That (edgesFlows.ContainsKey (originalEdge), Is.True);
				}
				);

				this.onComputationFinished (systemName, 
				                            finalState.FluidDynamicSystemStateMathematicallySolved.MutationResult);

			}
			#endregion

			protected virtual void visitNegativeLoadsCheckedSystemState (
			FluidDynamicSystemStateNegativeLoadsCorrected state,
			out Dictionary<GasNodeAbstract, double> unknownsByNodes, 
			out Dictionary<GasEdgeAbstract, double> QvaluesByEdges)
			{
				unknownsByNodes = new Dictionary<GasNodeAbstract, double> ();
				QvaluesByEdges = new Dictionary<GasEdgeAbstract, double> ();

				var dimensionalUnknowns = state.FluidDynamicSystemStateMathematicallySolved.
				MutationResult.makeUnknownsDimensional ().WrappedObject;

				var originalNodesBySubstitutedNodes = state.NodesSubstitutions.OriginalsBySubstituted ();
				foreach (var aNodePair in state.FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalNodesByComputationNodes) {

					var originalNode = originalNodesBySubstitutedNodes.ContainsKey (aNodePair.Value) ?
					originalNodesBySubstitutedNodes [aNodePair.Value] : aNodePair.Value;

					unknownsByNodes.Add (originalNode, dimensionalUnknowns.valueAt (aNodePair.Key));
				}

				var originalEdgesBySubstitutedNodes = state.EdgesSubstitutions.OriginalsBySubstituted ();
				foreach (var edgePair in state.FluidDynamicSystemStateMathematicallySolved.MutationResult.
			         StartingUnsolvedState.OriginalEdgesByComputationEdges) {

					var originalEdge = originalEdgesBySubstitutedNodes.ContainsKey (edgePair.Value) ?
					originalEdgesBySubstitutedNodes [edgePair.Value] : edgePair.Value; 

					QvaluesByEdges.Add (originalEdge,
				                    state.FluidDynamicSystemStateMathematicallySolved.
				                    MutationResult.Qvector.valueAt (edgePair.Key)
					);
				}

			}
		
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

