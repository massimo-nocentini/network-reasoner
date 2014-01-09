using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.terranova
{
	public class ComputationEntryPoint
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

			public Dictionary<GasNodeAbstract, System.Text.StringBuilder> AnomaliesByNodes {
				get;
				set;
			}

			public Dictionary<GasEdgeAbstract, System.Text.StringBuilder> AnomaliesByEdges {
				get;
				set;
			}
		}

		public virtual ComputationResults solve (
			string systemName,
			Dictionary<string, GasNodeAbstract> nodes,
			Dictionary<string, GasEdgeAbstract> edges,
			AmbientParameters ambientParameters,
			NetwonRaphsonSystemEventsListener eventListener,
			double precision)
		{
			var originalDomainReverterVisitor = 
				new FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain ();

			RunnableSystem runnable_system = new RunnableSystemComputeGivenEventListener{
				EventListener = eventListener,
				Precision = precision,
				UnknownInitialization = new UnknownInitializationSimplyRandomized()
			};

			runnable_system = new RunnableSystemWithDecorationApplySystemStateVisitor{
				DecoredRunnableSystem = runnable_system,
				SystemStateVisitor = originalDomainReverterVisitor
			};

			var aSystemState = runnable_system.compute (
				systemName, nodes, edges, ambientParameters);

			ComputationResults results = new ComputationResults ();

			results.RelativePressuresByNodes = originalDomainReverterVisitor.PressuresByNodes;
			results.AlgebraicSumOfFlowsByNodes = originalDomainReverterVisitor.AlgebraicSumOfFlowsByNodes;
			results.FlowsByEdges = originalDomainReverterVisitor.FlowsByEdges;
			results.VelocitiesByEdges = originalDomainReverterVisitor.VelocitiesByEdges;
			results.ElapsedTime = originalDomainReverterVisitor.ElapsedTime;
			results.AnomaliesByNodes = originalDomainReverterVisitor.AnomaliesByNodes;
			results.AnomaliesByEdges = originalDomainReverterVisitor.AnomaliesByEdges;

			return results;
		}


	}
}

