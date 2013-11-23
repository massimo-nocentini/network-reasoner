using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using System.IO;
using it.unifi.dsi.stlab.networkreasoner.gas.system;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface.tests
{
	[TestFixture()]
	public class SplittedGheoNetInputParsing
	{

		class CheckParsingForS04System : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;
								
				Assert.AreEqual ("s04", systemName);

				// implement here the assertions
				Assert.AreEqual (30, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (0, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (0, (N3.Gadget as GasNodeGadgetLoad).Load);

				return null;
			}
			#endregion
		}

		class CheckParsingForS05System : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;
								
				Assert.AreEqual ("s05", systemName);

				// implement here the assertions
				Assert.AreEqual (30, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (0, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (0, (N3.Gadget as GasNodeGadgetLoad).Load);

				return null;
			}
			#endregion
		}

		class CheckParsingForS06System : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;
								
				Assert.AreEqual ("s06", systemName);

				// implement here the assertions
				Assert.AreEqual (30, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (0, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (0, (N3.Gadget as GasNodeGadgetLoad).Load);

				return null;
			}
			#endregion
		}

		class CheckParsingForS07System : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;
								
				Assert.AreEqual ("s07", systemName);

				// implement here the assertions
				Assert.AreEqual (50, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (15, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (0, (N3.Gadget as GasNodeGadgetLoad).Load);

				return null;
			}
			#endregion
		}

		class CheckParsingForS08System : RunnableSystem
		{
			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				it.unifi.dsi.stlab.networkreasoner.model.gas.AmbientParameters ambientParameters)
			{
				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;
								
				Assert.AreEqual ("s08", systemName);

				// implement here the assertions
				Assert.AreEqual (50, (N1.Gadget as GasNodeGadgetSupply).SetupPressure);
				Assert.AreEqual (30, (N2.Gadget as GasNodeGadgetLoad).Load);
				Assert.AreEqual (0, (N3.Gadget as GasNodeGadgetLoad).Load);

				return null;
			}
			#endregion
		}

		class CheckAllSystems : RunnableSystem
		{
			Dictionary<String, RunnableSystem> CheckingSystems{ get; set; }

			public	CheckAllSystems ()
			{
				CheckingSystems = new Dictionary<string, RunnableSystem> ();

				CheckingSystems.Add ("s04", new CheckParsingForS04System ());
				CheckingSystems.Add ("s05", new CheckParsingForS05System ());
				CheckingSystems.Add ("s06", new CheckParsingForS06System ());
				CheckingSystems.Add ("s07", new CheckParsingForS07System ());
				CheckingSystems.Add ("s08", new CheckParsingForS08System ());
			}

			#region RunnableSystem implementation
			public FluidDynamicSystemStateAbstract compute (
				string systemName, 
				System.Collections.Generic.Dictionary<string, GasNodeAbstract> nodes, 
				System.Collections.Generic.Dictionary<string, GasEdgeAbstract> edges, 
				AmbientParameters ambientParameters)
			{
				this.CheckingSystems [systemName].compute (
					systemName, nodes, edges, ambientParameters);

				var N1 = nodes ["N1"] as GasNodeWithGadget;
				var N2 = nodes ["N2"] as GasNodeWithGadget;
				var N3 = nodes ["N3"] as GasNodeWithGadget;

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
				             Is.EqualTo (1.08e-5).Within (.0001));

				return null;
			}
			#endregion
		}

		[Test()]
		public void read_simple_specification_for_gheonet ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				new System.IO.FileInfo ("examples/first-simple-specification.dat"));

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted (
					new FileInfo ("examples/nodes-loads-pressures-for-splitted-specification.dat"))
			);

			systemRunner.run (new CheckAllSystems ());			
		}
	}
}

