using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.textualinterface;
using System.IO;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.unknowns_initializations;
using System.Linq;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.terranova.tests
{
	[TestFixture()]
	public class BigNetworkUsedDuringIntegrationTest
	{
		public class CheckingOriginalObjectMatching : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				Dictionary<string, GasNodeAbstract> nodes, 
				Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				var terranovaEntryPoint = new ComputationEntryPoint ();
				var terranovaResults = terranovaEntryPoint.solve (
					"big network for integration",
					nodes,
					edges,
					ambientParameters,
					new NetwonRaphsonSystemEventsListenerNullObject (),
					1e-6);

				edges.Values.ToList ().ForEach (anEdge => Assert.That (
					terranovaResults.FlowsByEdges.Keys, Contains.Item (anEdge)));

				edges.Values.ToList ().ForEach (anEdge => Assert.That (
					terranovaResults.VelocitiesByEdges.Keys, Contains.Item (anEdge)));

				nodes.Values.ToList ().ForEach (aNode => Assert.That (
					terranovaResults.RelativePressuresByNodes.Keys, Contains.Item (aNode)));

				nodes.Values.ToList ().ForEach (aNode => Assert.That (
					terranovaResults.AlgebraicSumOfFlowsByNodes.Keys, Contains.Item (aNode)));

				return null;
			}
			#endregion
		}

		[Test()]
		public void check_that_original_network_objects_appear_in_results ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("networks/big-network-for-integration.org"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerAllInOneFile ());

			systemRunner.run (new CheckingOriginalObjectMatching ());
		}
	}
}

