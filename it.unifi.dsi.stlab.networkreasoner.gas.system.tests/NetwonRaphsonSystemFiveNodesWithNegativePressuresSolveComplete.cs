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
using it.unifi.dsi.stlab.extension_methods;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using it.unifi.dsi.stlab.math.algebra;
using System.Linq;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemFiveNodesWithNegativePressuresSolveComplete
	{

		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets_with_splitted_specification ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("gheonet-textual-networks/five-nodes-network.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted (
					new FileInfo ("gheonet-textual-networks/five-nodes-network-extension.dat"))
			);

			RunnableSystem runnableSystem = new RunnableSystemCompute {
				LogConfigFileInfo = new FileInfo (
					"log4net-configurations/for-five-nodes-network-with-negative-pressures.xml"),
				Precision = 1e-4,
				UnknownInitialization = new UnknownInitializationSimplyRandomized ()
			};
			
			var originalDomainReverterVisitor = 
				new FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain ();

			runnableSystem = new RunnableSystemWithDecorationApplySystemStateVisitor{
				DecoredRunnableSystem = runnableSystem,
				SystemStateVisitor = originalDomainReverterVisitor 
			};

			Dictionary<String, GasNodeAbstract> nodes = null;
			Dictionary<String, GasEdgeAbstract> edges = null;
			runnableSystem = new RunnableSystemWithDecorationComputeCompletedHandler{
				DecoredRunnableSystem = runnableSystem,
				OnComputeStartedHandler = (systemName, originalNodes, originalEdges, ambientParameters) => {
					nodes = originalNodes;
					edges = originalEdges;
				}
			};

			systemRunner.run (runnableSystem);

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
		}
//
//		void checkIfOriginalObjectsAreStillPresent (
//			String systemName, FluidDynamicSystemStateAbstract aSystemState)
//		{
//			Assert.That (aSystemState, Is.InstanceOf (
//				typeof(FluidDynamicSystemStateNegativeLoadsCorrected))
//			);
//
//			var negativeLoadsCorrectedState = aSystemState as FluidDynamicSystemStateNegativeLoadsCorrected;
//
//
//
//		}

	}
}

