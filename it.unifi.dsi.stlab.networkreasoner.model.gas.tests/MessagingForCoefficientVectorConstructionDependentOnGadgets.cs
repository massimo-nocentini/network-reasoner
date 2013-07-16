using System;
using NUnit.Framework;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class MessagingForCoefficientVectorConstructionDependentOnGadgets
	{
		
		[Test()]
		public void equipping_a_node_with_a_load_it_produce_the_load_as_coefficient ()
		{
			long height = 40;
			GasNodeAbstract node = new GasNodeTopological {
				Identifier = "Dummy identifier",
				Height = height
			};

			double load = 74.98;
			GasNodeGadget gadget = new GasNodeGadgetLoad{ Load= load};

			GasNodeWithGadget wrapper = new GasNodeWithGadget ();
			wrapper.Equipped = node;
			wrapper.Gadget = gadget;

			// this is really the same order of operation that at the moment
			// I'll put in the final code.
			node = wrapper;


			// now we try to build the adapter for the matrix construction
			NodeMatrixConstruction nodeMatrixConstruction = 
				node.adapterForMatrixConstruction ();

			Assert.AreEqual (load, nodeMatrixConstruction.coefficient ());
		}

		[Test()]
		public void equipping_a_node_with_a_supply_it_produce_the_setup_pressure_as_coefficient ()
		{
			long height = 40;
			GasNodeAbstract node = new GasNodeTopological {
				Identifier = "Dummy identifier",
				Height = height
			};

			double setupPressure = 174.98;
			GasNodeGadget gadget = new GasNodeGadgetSupply{ 
				SetupPressure= setupPressure};

			GasNodeWithGadget wrapper = new GasNodeWithGadget ();
			wrapper.Equipped = node;
			wrapper.Gadget = gadget;

			// this is really the same order of operation that at the moment
			// I'll put in the final code.
			node = wrapper;

			// now we try to build the adapter for the matrix construction
			NodeMatrixConstruction nodeMatrixConstruction = 
				node.adapterForMatrixConstruction ();

			Assert.AreEqual (setupPressure, nodeMatrixConstruction.coefficient ());
		}
	}
}

