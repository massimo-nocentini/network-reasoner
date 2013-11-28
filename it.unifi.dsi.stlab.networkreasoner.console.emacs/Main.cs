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
using it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table;

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


		class DotRepresentationsRunnableSystem : RunnableSystem
		{
			Dictionary<string, StringBuilder> DotRepresentationsBySystems{ get; set; }

			public DotRepresentationsRunnableSystem ()
			{
				DotRepresentationsBySystems = new Dictionary<string, StringBuilder> ();
			}

			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
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

				return null;
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

		protected virtual void buildSystemRunner (
			TextualGheoNetInputParser parser, 
			out SystemRunnerFromTextualGheoNetInput systemRunner)
		{
			var multirun_region_identifier = "multirun";

			if (parser.existsLineSuchThat (line => line.StartsWith ("* " + multirun_region_identifier))) {
				var multirun_region = parser.fetchRegion (
					multirun_region_identifier, new TableHeaderParserKeepHeaderRow ());

				systemRunner = parser.parse (new SpecificationAssemblerSplitted (multirun_region));
			} else {
				systemRunner = parser.parse (new SpecificationAssemblerAllInOneFile ());
			}
		}

		public void run (List<String> lines)
		{
			TextualGheoNetInputParser parser = 
				new TextualGheoNetInputParser (lines);

			SystemRunnerFromTextualGheoNetInput systemRunner = null;

			buildSystemRunner (parser, out systemRunner);

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
				log = LogManager.GetLogger (typeof(object));			
				XmlConfigurator.Configure (new FileInfo (logRow [1]));

				if (listener is NetwonRaphsonSystemEventsListenerForLogging) {
					(listener as NetwonRaphsonSystemEventsListenerForLogging).Log = log;
				}
			}

			RunnableSystem runnable_system = new RunnableSystemComputeGivenEventListener{
				EventListener = listener,
				Precision = precision,
				UnknownInitialization = new UnknownInitializationSimplyRandomized()
			};

			var summaryTableVisitor = new FluidDynamicSystemStateVisitorBuildSummaryTable ();
			runnable_system = new RunnableSystemWithDecorationApplySystemStateVisitor{
				DecoredRunnableSystem = runnable_system,
				SystemStateVisitor = summaryTableVisitor
			};

			systemRunner.run (runnable_system);
			
			Console.WriteLine (string.Format ("* steady state analysis\n{0}", 
			                                  summaryTableVisitor.buildSummaryContent ())
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
