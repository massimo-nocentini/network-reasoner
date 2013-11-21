using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using log4net;
using log4net.Config;
using System.IO;
using System.Linq;
using it.unifi.dsi.stlab.extensionmethods;
using System.Globalization;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NewtonRaphsonSystem
	{
		public virtual void solve (
			GasNetwork aGasNetwork,
			AmbientParameters ambientParameters,
			NetwonRaphsonSystemEventsListener eventListener,
			double precision,
			out Dictionary<GasNodeAbstract, double> pressuresByNodes, 
			out Dictionary<GasEdgeAbstract, double> flowsByEdges)
		{
			var translatorMaker = new dimensional_objects.DimensionalDelegates ();

			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned ();
			formulaVisitor.AmbientParameters = ambientParameters;

			var initializationTransition = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
			initializationTransition.EventsListener = eventListener;
			initializationTransition.Network = aGasNetwork;
			initializationTransition.UnknownInitialization = new UnknownInitializationSimplyRandomized ();
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
					Precision = precision
				}};
			
			var negativeLoadsCheckerTransition = new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
			negativeLoadsCheckerTransition.EventsListener = eventListener;
			negativeLoadsCheckerTransition.FormulaVisitor = formulaVisitor;

			var system = new FluidDynamicSystemStateTransitionCombinator ();
			var finalState = system.applySequenceOnBareState (new List<FluidDynamicSystemStateTransition>{
				initializationTransition, solveTransition}
			);

			var originalDomainReverterVisitor = new FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain ();
			finalState.accept (originalDomainReverterVisitor);

			pressuresByNodes = originalDomainReverterVisitor.PressuresByNodes;
			flowsByEdges = originalDomainReverterVisitor.FlowsByEdges;
		}


	}
}

