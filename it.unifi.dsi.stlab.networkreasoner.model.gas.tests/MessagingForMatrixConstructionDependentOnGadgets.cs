using System;
using NUnit.Framework;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class MessagingForMatrixConstructionDependentOnGadgets
	{
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

