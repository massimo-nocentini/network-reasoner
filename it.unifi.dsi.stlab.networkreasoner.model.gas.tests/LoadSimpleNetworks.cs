using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class LoadSimpleNetworks
	{
		[Test()]
		public void load_simple_network_and_check_if_main_network_has_been_populated ()
		{
			GasNetwork network = new GasNetwork ();

			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			loader.Load ("../../nturtle-specifications/specification-for-loading-a-simple-gas-network.nt",
			            network.ParserResultReceiver);

			Assert.IsNotNull (network);
			Assert.AreEqual (4, network.Edges.Count);
			Assert.AreEqual (4, network.Nodes.Count);
		}

//		[Test()]
//		public void equipping_a_node_preserve_properties ()
//		{
//			long height = 40;
//			GasNodeAbstract basicNode = new GasNodeTopological {
//				Identifier = "Dummy identifier",
//				Height = height
//			};
//
//			double load = 74.98;
//			GasNodeGadget gadget = new GasNodeGadgetLoad{ Load= load};
//
//			GasNodeWithGadget wrapper = new GasNodeWithGadget ();
//			wrapper.Equipped = basicNode;
//			wrapper.Gadget = gadget;
//
//			// this is really the same order of operation that at the moment
//			// I'll put in the final code.
//			basicNode = wrapper;
//
//			// just check that the wrapping preserve the properties
//			// of the equipped node (aka. the wrappee)
//			Assert.AreEqual (height, basicNode.Height);
//		}

		[Test()]
		public void load_a_complete_network_with_a_main_object_defined_in_specification ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-ambient-parameters.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network.AmbientParameters);
			Assert.AreEqual ("Methane", network.AmbientParameters.ElementName);
			Assert.AreEqual (1013.25, network.AmbientParameters.RefPressureInBar);
			Assert.AreEqual (1013.25, network.AmbientParameters.AirPressureInBar);
			Assert.AreEqual (15.0, network.AmbientParameters.ElementTemperatureInKelvin);
			Assert.AreEqual (15.0, network.AmbientParameters.AirTemperatureInKelvin);
			Assert.AreEqual (16.0, network.AmbientParameters.MolWeight);
			Assert.AreEqual (0.0108, network.AmbientParameters.ViscosityInPascalTimesSecond);

		}

	}
}

