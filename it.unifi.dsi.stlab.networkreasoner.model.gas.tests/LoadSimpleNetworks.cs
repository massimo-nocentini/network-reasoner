using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

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
			            network.parserResultReceiver ());

			Assert.AreEqual (4, network.Edges.Count);
			Assert.AreEqual (4, network.Nodes.Count);
		}

		[Test()]
		public void equipping_a_node_preserve_properties ()
		{
			long height = 40;
			GasNodeAbstract basicNode = new GasNode {
				Identifier = "Dummy identifier",
				Height = height
			};

			double load = 74.98;
			GasNodeGadget gadget = new GasNodeGadgetLoad{ Load= load};

			GasNodeWithGadget wrapper = new GasNodeWithGadget ();
			wrapper.Equipped = basicNode;
			wrapper.Gadget = gadget;

			// this is really the same order of operation that at the moment
			// I'll put in the final code.
			basicNode = wrapper;

			// just check that the wrapping preserve the properties
			// of the equipped node (aka. the wrappee)
			Assert.AreEqual (height, basicNode.Height);
		}

		[Test()]
		public void equipping_a_node_with_a_load_it_produce_the_load_as_coefficient ()
		{
			long height = 40;
			GasNodeAbstract node = new GasNode {
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
			GasNodeAbstract node = new GasNode {
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

		[Test()]
		public void equipping_a_node_with_a_supply_it_returns_one_only_for_its_diagonal_position_in_matrix ()
		{
			long height = 40;
			GasNodeAbstract node = new GasNode {
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

			// we really don't care about setting meaningful data here
			// this node serve only to represent a new node object 
			// with a completely different identity from the equipped one.
			GasNodeAbstract anotherNode = new GasNode{ Identifier="another identifier"};
			GasNodeGadget gadgetForAnotherNode = new GasNodeGadgetLoad{ 
				Load= 38};
			GasNodeWithGadget wrapperForAnotherNode = new GasNodeWithGadget ();
			wrapperForAnotherNode.Equipped = anotherNode;
			wrapperForAnotherNode.Gadget = gadgetForAnotherNode;
			anotherNode = wrapperForAnotherNode;

			// now we try to build the adapter for the matrix construction
			NodeMatrixConstruction nodeMatrixConstruction = 
				node.adapterForMatrixConstruction ();

			Assert.AreEqual (1, 
			                 nodeMatrixConstruction.matrixValueRespect (
				nodeMatrixConstruction,
				new DummyMatrixComputationDataProvider ())
			);

			Assert.AreEqual (0, 
			                 nodeMatrixConstruction.matrixValueRespect (
				anotherNode.adapterForMatrixConstruction (),
				new DummyMatrixComputationDataProvider ())
			);
		}

		[Test()]
		public void equipping_a_node_with_a_load_it_returns_little_ks_for_both_columns_position_in_matrix ()
		{
			long height = 40;
			GasNodeAbstract node = new GasNode {
				Identifier = "Dummy identifier",
				Height = height
			};

			double setupPressure = 174.98;
			GasNodeGadget gadget = new GasNodeGadgetSupply{ 
				SetupPressure= setupPressure};

			GasNodeWithGadget wrapper = new GasNodeWithGadget ();
			wrapper.Equipped = node;
			wrapper.Gadget = gadget;
			node = wrapper;

			// we really don't care about setting meaningful data here
			// this node serve only to represent a new node object 
			// with a completely different identity from the equipped one.
			GasNodeAbstract anotherNode = new GasNode{ Identifier="another identifier"};
			GasNodeGadget gadgetForAnotherNode = new GasNodeGadgetLoad{ 
				Load= 38};
			GasNodeWithGadget wrapperForAnotherNode = new GasNodeWithGadget ();
			wrapperForAnotherNode.Equipped = anotherNode;
			wrapperForAnotherNode.Gadget = gadgetForAnotherNode;
			anotherNode = wrapperForAnotherNode;

			// now we try to build the adapter for the matrix construction
			NodeMatrixConstruction nodeMatrixConstruction = 
				anotherNode.adapterForMatrixConstruction ();

			double value = 4837.209;
			var dummyMatrixComputationDataProvider = new DummyMatrixComputationDataProvider ();
			dummyMatrixComputationDataProvider.Value = value;

			Assert.AreEqual (value, 
			                 nodeMatrixConstruction.matrixValueRespect (
				nodeMatrixConstruction,
				dummyMatrixComputationDataProvider)
			);

			Assert.AreEqual (-1 * value, 
			                 nodeMatrixConstruction.matrixValueRespect (
				node.adapterForMatrixConstruction (),
				dummyMatrixComputationDataProvider)
			);
		}

		private class DummyMatrixComputationDataProvider : MatrixComputationDataProvider
		{
			public double Value { get; set; }

			#region MatrixComputationDataProvider implementation
			public double LittleK (NodeMatrixConstruction startNode, 
			                       NodeMatrixConstruction endNode)
			{
				return Value;
			}
			#endregion
		}
	}
}

