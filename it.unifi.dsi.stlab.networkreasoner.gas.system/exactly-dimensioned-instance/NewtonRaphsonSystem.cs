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
		public class ComputationResults
		{
			public TimeSpan? ElapsedTime {
				get;
				set;
			}

			public Dictionary<GasNodeAbstract, double> RelativePressuresByNodes {
				get;
				set;
			}

			public Dictionary<GasNodeAbstract, double> AlgebraicSumOfFlowsByNodes {
				get;
				set;
			}

			public Dictionary<GasEdgeAbstract, double> FlowsByEdges {
				get;
				set;
			}

			public Dictionary<GasEdgeAbstract, double> VelocitiesByEdges {
				get;
				set;
			}
		}

		public virtual ComputationResults solve (
			GasNetwork aGasNetwork,
			AmbientParameters ambientParameters,
			NetwonRaphsonSystemEventsListener eventListener,
			double precision)
		{
			var formulaVisitor = new GasFormulaVisitorExactlyDimensioned ();
			formulaVisitor.AmbientParameters = ambientParameters;

			var initializationTransition = new FluidDynamicSystemStateTransitionInitializationRaiseEventsDecorator ();
			initializationTransition.EventsListener = eventListener;
			initializationTransition.Network = aGasNetwork;
			initializationTransition.UnknownInitialization = new UnknownInitializationSimplyRandomized ();

			var solveTransition = new FluidDynamicSystemStateTransitionNewtonRaphsonSolveRaiseEventsDecorator ();
			solveTransition.EventsListener = eventListener;
			solveTransition.FormulaVisitor = formulaVisitor;

			solveTransition.UntilConditions = new List<UntilConditionAbstract> {
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = precision
				}};
			
			var negativeLoadsCheckerTransition = 
				new FluidDynamicSystemStateTransitionNegativeLoadsCheckerRaiseEventsDecorator ();
			negativeLoadsCheckerTransition.EventsListener = eventListener;

			var system = new FluidDynamicSystemStateTransitionCombinator ();
			var finalState = system.applySequenceOnBareState (new List<FluidDynamicSystemStateTransition>{
				initializationTransition, solveTransition, negativeLoadsCheckerTransition}
			);

			var originalDomainReverterVisitor = 
				new FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain ();
			finalState.accept (originalDomainReverterVisitor);

			ComputationResults results = new ComputationResults ();

			results.RelativePressuresByNodes = originalDomainReverterVisitor.PressuresByNodes;
			results.AlgebraicSumOfFlowsByNodes = originalDomainReverterVisitor.AlgebraicSumOfFlowsByNodes;
			results.FlowsByEdges = originalDomainReverterVisitor.FlowsByEdges;
			results.VelocitiesByEdges = originalDomainReverterVisitor.VelocitiesByEdges;
			results.ElapsedTime = originalDomainReverterVisitor.ElapsedTime;

			return results;
		}


	}
}

