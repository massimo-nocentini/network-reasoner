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
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemTerranovaStubNetwork
	{
		class FiveNodesNetworkRunnableSystem : RunnableSystemAbstractComputationalResultHandlerShortTableSummary
		{
			Action<string, OneStepMutationResults> ActionWithAsserts{ get; set; }

			public FiveNodesNetworkRunnableSystem (Action<string, OneStepMutationResults> asserts)
			{
				this.ActionWithAsserts = asserts;
			}

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
				"log4net-configurations/terranova-stub-network.xml")
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
					Precision = 1e-8
				}};
			
				var negativeLoadsCheckerTransition = new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
				negativeLoadsCheckerTransition.EventsListener = eventListener;
				negativeLoadsCheckerTransition.FormulaVisitor = formulaVisitor;

				var system = new FluidDynamicSystemStateTransitionCombinator ();
				var finalState = system.applySequenceOnBareState (new List<FluidDynamicSystemStateTransition>{
				initializationTransition, solveTransition, negativeLoadsCheckerTransition}
				) as FluidDynamicSystemStateNegativeLoadsCorrected;

				var results = finalState.FluidDynamicSystemStateMathematicallySolved.MutationResult;

				this.onComputationFinished (systemName, results);

				// here we perform the necessary tests using the plugged behaviour
				this.ActionWithAsserts.Invoke (systemName, results);
			}
			#endregion
		}


		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo("gheonet-textual-networks/terranova-stub-network.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerAllInOneFile ()
			);

			var fiveNodesNetworkRunnableSystem = new FiveNodesNetworkRunnableSystem (
				assertsForSpecificationAllInOneFile);
			systemRunner.run (fiveNodesNetworkRunnableSystem);

			
			File.WriteAllText ("gheonet-textual-networks/terranova-stub-network-output.dat", 
			                  fiveNodesNetworkRunnableSystem.buildTableSummary ());
			
		}

		#region assertions for the single system relative to all-in-one file specification

		// this method contains assertions for the test above.
		void assertsForSpecificationAllInOneFile (
			string systemName, OneStepMutationResults results)
		{
			var relativeUnknowns = results.makeUnknownsDimensional ();

//			var node1 = results.findNodeByIdentifier ("N1");
//			var node2 = results.findNodeByIdentifier ("N2");
//			var node3 = results.findNodeByIdentifier ("N3");
//			var node4 = results.findNodeByIdentifier ("N4");
//			Assert.That (relativeUnknowns.valueAt (node1), Is.EqualTo (30.000).Within (1e-3));
//			Assert.That (relativeUnknowns.valueAt (node2), Is.EqualTo (27.792).Within (1e-3));
//			Assert.That (relativeUnknowns.valueAt (node3), Is.EqualTo (25.527).Within (1e-3));
//			Assert.That (relativeUnknowns.valueAt (node4), Is.EqualTo (27.016).Within (1e-3));
//
//			
//			var edgeR1 = results.findEdgeByIdentifier ("R1");
//			var edgeR2 = results.findEdgeByIdentifier ("R2");
//			var edgeR3 = results.findEdgeByIdentifier ("R3");
//			Assert.That (results.Qvector.valueAt (edgeR1), Is.EqualTo (200.000).Within (1e-3));
//			Assert.That (results.Qvector.valueAt (edgeR2), Is.EqualTo (-100.000).Within (1e-3));
//			Assert.That (results.Qvector.valueAt (edgeR3), Is.EqualTo (-100.000).Within (1e-3));
//			
		}

		#endregion

	}
}

