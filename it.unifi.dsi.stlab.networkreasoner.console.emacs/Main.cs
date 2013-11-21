using System;
using System.Collections.Generic;
using System.IO;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;
using System.Dynamic.Utils;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using log4net;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.console.emacs
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			var lines = new List<string> ();
			while (Console.In.Peek() != -1) {
				string input = Console.In.ReadLine ();
				lines.Add (input);
			}

			new MainClass ().run (lines);
		}

		class RunnableSystemWithSummaryTableDecoration : 
		RunnableSystemAbstractComputationalResultHandlerShortTableSummary
		{
			public NetwonRaphsonSystemEventsListener EventListener { get; set; }

			public Double Precision { get; set; }

			public override void compute (
				string systemName, 
				Dictionary<string, GasNodeAbstract> nodes, 
				Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				var aGasNetwork = new GasNetwork{
					Nodes = nodes,
					Edges = edges,				
					AmbientParameters = ambientParameters
				};

				var formulaVisitor = new GasFormulaVisitorExactlyDimensioned ();
				formulaVisitor.AmbientParameters = ambientParameters;

				var translatorMaker = new DimensionalDelegates ();

				var initializationTransition = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
				initializationTransition.EventsListener = EventListener;
				initializationTransition.Network = aGasNetwork;
				initializationTransition.UnknownInitialization = new UnknownInitializationSimplyRandomized ();
				initializationTransition.FromDimensionalToAdimensionalTranslator = 
				translatorMaker.throwExceptionIfThisTranslatorIsCalled<double> (
				"dimensional -> adimensional translation requested when it isn't required.");

				var solveTransition = new FluidDynamicSystemStateTransitionNewtonRaphsonSolveRaiseEventsDecorator ();
				solveTransition.EventsListener = EventListener;
				solveTransition.FormulaVisitor = formulaVisitor;
				solveTransition.FromDimensionalToAdimensionalTranslator = 
				translatorMaker.throwExceptionIfThisTranslatorIsCalled<Vector<NodeForNetwonRaphsonSystem>> (
				"dimensional -> adimensional translation requested when it isn't required.");
				solveTransition.UntilConditions = new List<UntilConditionAbstract> {
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = Precision
				}};
			
				var negativeLoadsCheckerTransition = new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
				negativeLoadsCheckerTransition.EventsListener = EventListener;
				negativeLoadsCheckerTransition.FormulaVisitor = formulaVisitor;

				var system = new FluidDynamicSystemStateTransitionCombinator ();
				var finalState = system.applySequenceOnBareState (new List<FluidDynamicSystemStateTransition>{
				initializationTransition, solveTransition, negativeLoadsCheckerTransition}
				) as FluidDynamicSystemStateNegativeLoadsCorrected;

				var results = finalState.FluidDynamicSystemStateMathematicallySolved.MutationResult;

				this.onComputationFinished (systemName, results);
			}
		}

		class DotRepresentationsRunnableSystem : RunnableSystem
		{
			Dictionary<string, StringBuilder> DotRepresentationsBySystems{ get; set; }

			public DotRepresentationsRunnableSystem ()
			{
				DotRepresentationsBySystems = new Dictionary<string, StringBuilder> ();
			}

			#region RunnableSystem implementation
			public void compute (
				string systemName, 
				Dictionary<string, GasNodeAbstract> nodes, 
				Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				var aGasNetwork = new GasNetwork{
					Nodes = nodes,
					Edges = edges,				
					AmbientParameters = ambientParameters
				};

				var dotRepresentationValidator = new DotRepresentationValidator ();
				var dotContent = dotRepresentationValidator.generateContent (aGasNetwork);

				DotRepresentationsBySystems.Add (systemName, dotContent);
			}
			#endregion

			public String buildDotRepresentations ()
			{
				StringBuilder result = new StringBuilder ();

				foreach (var pair in DotRepresentationsBySystems) {
					result.AppendFormat ("** dot representation for system {0}\n\t", pair.Key);
					result.AppendFormat ("{0}\n", pair.Value.ToString ());
				}

				return result.ToString ();
			}

		}

		public void run (List<String> lines)
		{
			TextualGheoNetInputParser parser = 
				new TextualGheoNetInputParser (lines);

			SystemRunnerFromTextualGheoNetInput systemRunner = null;
			var multirun_region_identifier = "multirun";

			if (lines.Exists (line => line.StartsWith ("* " + multirun_region_identifier))) {

				var multirun_region = parser.fetchRegion (multirun_region_identifier, 
				                                          new TableHeaderParserKeepHeaderRow ());

				systemRunner = parser.parse (new SpecificationAssemblerSplitted (multirun_region));
			} else {
				systemRunner = parser.parse (new SpecificationAssemblerAllInOneFile ());
			}

			var computationParametersRegion = parser.fetchRegion ("computation parameters", 
			                                                      new TableHeaderParserIgnoreHeader ());
			var precisionRow = parser.splitOrgRow (computationParametersRegion [0]);
			double precision = Double.MinValue;
			if (precisionRow.Length > 1) {
				precision = parser.parseDoubleCultureInvariant (precisionRow [1]).Value;
			}			

			var listenerRow = parser.splitOrgRow (computationParametersRegion [1]);
			NetwonRaphsonSystemEventsListener listener = null;
			if (listenerRow.Length > 1) {
				listener = Activator.CreateInstance (Type.GetType (listenerRow [1]))
					as NetwonRaphsonSystemEventsListener;
			} else {
				listener = new NetwonRaphsonSystemEventsListenerNullObject ();
			}

			var logRow = parser.splitOrgRow (computationParametersRegion [2]);
			ILog log = null;
			if (logRow.Length > 1) {
				log = LogManager.GetLogger (typeof(NewtonRaphsonSystem));			
				XmlConfigurator.Configure (new FileInfo (logRow [1]));

				if (listener is NetwonRaphsonSystemEventsListenerForLogging) {
					(listener as NetwonRaphsonSystemEventsListenerForLogging).Log = log;
				}
			}

			RunnableSystemAbstractComputationalResultHandlerShortTableSummary runnable_system = 
			new RunnableSystemWithSummaryTableDecoration{
				Precision = precision,
				EventListener = listener
			};

			systemRunner.run (runnable_system);
			
			Console.WriteLine (string.Format ("* steady state analysis\n{0}", 
			                                  runnable_system.buildSummaryContent ())
			);

			var dotRepresentationRow = parser.splitOrgRow (computationParametersRegion [3]);
			if (dotRepresentationRow.Length > 1) {
				var dotRepresentationsRunnableSystem = new DotRepresentationsRunnableSystem ();
				systemRunner.run (dotRepresentationsRunnableSystem);

				Console.WriteLine (string.Format ("\n\n* Dot representations\n{0}",
				                                  dotRepresentationsRunnableSystem.buildDotRepresentations ())
				);
			}

		}
	}
}
