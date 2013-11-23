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
using it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemSzoplikNetwork
	{

		[Test()]
		public void simple_network_with_potential_negative_pressure_for_nodes_with_load_gadgets ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("gheonet-textual-networks/szoplik-network.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerAllInOneFile ()
			);

			RunnableSystem runnableSystem = new RunnableSystemCompute {
				LogConfigFileInfo = new FileInfo (
					"log4net-configurations/szoplik-network.xml"),
				Precision = 1e-8,
				UnknownInitialization = new UnknownInitializationSimplyRandomized ()
			};

			runnableSystem = new RunnableSystemWithDecorationComputeCompletedHandler{
				DecoredRunnableSystem = runnableSystem,
				OnComputeCompletedHandler = checkAssertions
			};

			var summaryTableVisitor = new FluidDynamicSystemStateVisitorBuildSummaryTable ();
			runnableSystem = new RunnableSystemWithDecorationApplySystemStateVisitor{
				DecoredRunnableSystem = runnableSystem,
				SystemStateVisitor = summaryTableVisitor
			};

			systemRunner.run (runnableSystem);

			File.WriteAllText ("gheonet-textual-networks/szoplik-network-output.dat", 
			                  summaryTableVisitor.buildSummaryContent ());
			
		}

		#region assertions for the single system relative to all-in-one file specification
		void checkAssertions (String systemName, FluidDynamicSystemStateAbstract aSystemState)
		{
			Assert.That (aSystemState, Is.InstanceOf (
				typeof(FluidDynamicSystemStateNegativeLoadsCorrected))
			);

			OneStepMutationResults results = 
				(aSystemState as FluidDynamicSystemStateNegativeLoadsCorrected).
					FluidDynamicSystemStateMathematicallySolved.MutationResult;

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

