using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.IO;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using log4net;
using log4net.Config;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.networkreasoner.gas.system;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class RunnableSystemCompute : RunnableSystem
	{
			
		public FileInfo LogConfigFileInfo{ get; set; }

		public Double Precision { get; set; }

		public UnknownInitialization UnknownInitialization { get; set; }

		public RunnableSystemCompute ()
		{
			// if we forget to initialize the precision, then we compute with 
			// the maximum accuracy available (which can take a long time however)
			Precision = double.MinValue;
		}

			#region RunnableSystem implementation
		public FluidDynamicSystemStateAbstract compute (
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

			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned ();
			formulaVisitor.AmbientParameters = ambientParameters;

			var eventListener = buildEventListener ();

			var initializationTransition = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
			initializationTransition.EventsListener = eventListener;
			initializationTransition.Network = aGasNetwork;
			initializationTransition.UnknownInitialization = UnknownInitialization;

			var solveTransition = new FluidDynamicSystemStateTransitionNewtonRaphsonSolveRaiseEventsDecorator ();
			solveTransition.EventsListener = eventListener;
			solveTransition.FormulaVisitor = formulaVisitor;
			solveTransition.UntilConditions = buildUntilConditions (); 
			
			var negativeLoadsCheckerTransition = new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
			negativeLoadsCheckerTransition.EventsListener = eventListener;

			var transitionsCombination = new FluidDynamicSystemStateTransitionCombinator ();
			var transitionsSequence = buildTransitionsSequence (
					initializationTransition, 
					solveTransition, 
					negativeLoadsCheckerTransition);

			// return the final state when refactoring finishes
			var aSystemState = transitionsCombination.applySequenceOnBareState (transitionsSequence);

			return aSystemState;
		}
			#endregion

		protected virtual List<UntilConditionAbstract> buildUntilConditions ()
		{
			return new List<UntilConditionAbstract> {
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = Precision
					}};
		}

		protected virtual List<FluidDynamicSystemStateTransition> buildTransitionsSequence (
				FluidDynamicSystemStateTransitionInitialization initializationTransition,
				FluidDynamicSystemStateTransitionNewtonRaphsonSolve solveTransition,
				FluidDynamicSystemStateTransitionNegativeLoadsChecker negativeLoadsCheckerTransition)
		{
			return new List<FluidDynamicSystemStateTransition>{
					initializationTransition, solveTransition, negativeLoadsCheckerTransition};
		}

		protected virtual NetwonRaphsonSystemEventsListener buildEventListener ()
		{		
			ILog log = LogManager.GetLogger (typeof(Object));
			XmlConfigurator.Configure (LogConfigFileInfo);

			var eventListener = new NetwonRaphsonSystemEventsListenerForLoggingSummary ();
			eventListener.Log = log;

			return eventListener;
		}
	}
}

