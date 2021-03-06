using System;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface.tests
{
	[TestFixture()]
	public class SimpleGheoNetInputParsing
	{
		class CheckParsing : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				String systemName,
				Dictionary<string, GasNodeAbstract> nodes, 
				Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;

				// implement here the assertions
				Assert.AreEqual (200, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (100, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (50, (N3.Gadget as GasNodeGadgetLoad).Load);

				var R1 = edges ["R1"] as GasEdgePhysical;
				var R2 = edges ["R2"] as GasEdgePhysical;
				var R3 = edges ["R3"] as GasEdgePhysical;

				Assert.AreSame (N1, (R1.Described as GasEdgeTopological).StartNode);
				Assert.AreSame (N2, (R1.Described as GasEdgeTopological).EndNode);
				Assert.AreEqual (80, R1.Diameter);
				Assert.AreEqual (100, R1.Length);
				Assert.AreEqual (.5, R1.Roughness);

				Assert.AreSame (N2, (R2.Described as GasEdgeTopological).StartNode);
				Assert.AreSame (N3, (R2.Described as GasEdgeTopological).EndNode);
				Assert.AreEqual (80, R2.Diameter);
				Assert.AreEqual (200, R2.Length);
				Assert.AreEqual (.5, R2.Roughness);

				Assert.AreSame (N3, (R3.Described as GasEdgeTopological).StartNode);
				Assert.AreSame (N1, (R3.Described as GasEdgeTopological).EndNode);
				Assert.AreEqual (80, R3.Diameter);
				Assert.AreEqual (200, R3.Length);
				Assert.AreEqual (.5, R3.Roughness);

				Assert.AreEqual (1.01325, ambientParameters.AirPressureInBar);
				Assert.AreEqual (288.15, ambientParameters.AirTemperatureInKelvin);
				Assert.AreEqual (288.15, ambientParameters.ElementTemperatureInKelvin);
				Assert.AreEqual (16, ambientParameters.MolWeight);
				Assert.AreEqual (1.01325, ambientParameters.RefPressureInBar);
				Assert.AreEqual (288.15, ambientParameters.RefTemperatureInKelvin);
				Assert.That (ambientParameters.ViscosityInPascalTimesSecond, 
				             Is.EqualTo (1.08e-5).Within (1e-5));

				return null;
			}
			#endregion
		}

		[Test()]
		public void read_simple_specification_for_gheonet ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new FileInfo ("examples/first-simple-specification.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerAllInOneFile ());

			systemRunner.run (new CheckParsing ());			
		}


	}
}

