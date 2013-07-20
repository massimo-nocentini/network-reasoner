using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NodeForNetwonRaphsonSystemCoefficientVector
	{
		[Test()]
		public void building_a_vector_from_one_supplier_only_should_produce_a_vector_with_one_item_equals_to_setup_pressure ()
		{
			var aSetupPressure = 387.291;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 0, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { aSetupPressure });

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_a_supplier_against_three_other_nodes_should_produce_a_vector_of_length_four ()
		{
			var aSetupPressure = 387.291;
			var defaultForOthers = 0;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological ());

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological ());

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological ());

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode1, 0, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode2, 1, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 2, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode3, 3, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				aSetupPressure,
				defaultForOthers
			}
			);

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_two_suppliers_against_three_other_nodes_should_produce_a_vector_of_length_five ()
		{
			var aSetupPressure1 = 387.291;
			var aSetupPressure2 = 3.29;
			
			var defaultForOthers = 0;

			var aSupplier1 = new GasNodeWithGadget ();
			aSupplier1.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure1 };
			aSupplier1.Equipped = new GasNodeTopological ();

			var nodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem1.initializeWith (aSupplier1);

			var aSupplier2 = new GasNodeWithGadget ();
			aSupplier2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure2 };
			aSupplier2.Equipped = new GasNodeTopological ();

			var nodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem2.initializeWith (aSupplier2);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem1.putYourCoefficientInto (aVector);
			nodeForSystem2.putYourCoefficientInto (aVector);
			
			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological ());

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological ());

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological ());

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode1, 0, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode2, 1, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem1, 2, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode3, 3, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem2, 4, aDouble => aDouble));
			
			var a = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				aSetupPressure1,
				defaultForOthers,
				aSetupPressure2
			}
			);

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_one_loader_only_should_produce_a_vector_with_one_item_equals_to_gadget_load ()
		{
			var aLoad = 387.291;

			var aLoader = new GasNodeWithGadget ();
			aLoader.Gadget = new GasNodeGadgetLoad{ Load = aLoad };
			aLoader.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aLoader);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 0, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { aLoad });

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_a_loader_against_three_other_nodes_should_produce_a_vector_of_length_four ()
		{
			var aLoad = 387.291;
			var defaultForOthers = 0;

			var aLoader = new GasNodeWithGadget ();
			aLoader.Gadget = new GasNodeGadgetLoad{ Load = aLoad };
			aLoader.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aLoader);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological ());

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological ());

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological ());

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode1, 0, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode2, 1, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 2, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode3, 3, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				aLoad,
				defaultForOthers
			}
			);

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_two_loaders_against_three_other_nodes_should_produce_a_vector_of_length_five ()
		{
			var aLoad1 = 387.291;
			var aLoad2 = 3.29;
			
			var defaultForOthers = 0;

			var aLoader1 = new GasNodeWithGadget ();
			aLoader1.Gadget = new GasNodeGadgetSupply{ SetupPressure = aLoad1 };
			aLoader1.Equipped = new GasNodeTopological ();

			var nodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem1.initializeWith (aLoader1);

			var aLoader2 = new GasNodeWithGadget ();
			aLoader2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aLoad2 };
			aLoader2.Equipped = new GasNodeTopological ();

			var nodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem2.initializeWith (aLoader2);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem1.putYourCoefficientInto (aVector);
			nodeForSystem2.putYourCoefficientInto (aVector);
			
			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological ());

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological ());

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological ());

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode1, 0, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode2, 1, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem1, 2, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (anotherNode3, 3, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem2, 4, aDouble => aDouble));
			
			var a = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				aLoad1,
				defaultForOthers,
				aLoad2
			}
			);

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_one_passive_node_only_should_produce_a_vector_with_one_item_equals_to_default_load ()
		{
			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (new GasNodeTopological ());

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 0, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { 0.0 });

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void mixing_all_together ()
		{
			var aSetupPressure1 = 387.291;
			var aSetupPressure2 = 3.29;
			

			var aSupplier1 = new GasNodeWithGadget ();
			aSupplier1.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure1 };
			aSupplier1.Equipped = new GasNodeTopological ();

			var supplyNodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			supplyNodeForSystem1.initializeWith (aSupplier1);

			var aSupplier2 = new GasNodeWithGadget ();
			aSupplier2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure2 };
			aSupplier2.Equipped = new GasNodeTopological ();

			var supplyNodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			supplyNodeForSystem2.initializeWith (aSupplier2);



			var aLoad1 = 387.291;
			var aLoad2 = 3.29;
			

			var aLoader1 = new GasNodeWithGadget ();
			aLoader1.Gadget = new GasNodeGadgetSupply{ SetupPressure = aLoad1 };
			aLoader1.Equipped = new GasNodeTopological ();

			var loadNodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			loadNodeForSystem1.initializeWith (aLoader1);

			var aLoader2 = new GasNodeWithGadget ();
			aLoader2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aLoad2 };
			aLoader2.Equipped = new GasNodeTopological ();

			var loadNodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			loadNodeForSystem2.initializeWith (aLoader2);

			var passiveNodeForSystem = new NodeForNetwonRaphsonSystem ();
			passiveNodeForSystem.initializeWith (new GasNodeTopological ());

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			loadNodeForSystem1.putYourCoefficientInto (aVector);
			loadNodeForSystem2.putYourCoefficientInto (aVector);
			supplyNodeForSystem1.putYourCoefficientInto (aVector);
			supplyNodeForSystem2.putYourCoefficientInto (aVector);
			passiveNodeForSystem.putYourCoefficientInto (aVector);

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (loadNodeForSystem1, 0, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (supplyNodeForSystem2, 1, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (passiveNodeForSystem, 2, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (loadNodeForSystem2, 3, aDouble => aDouble));
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (supplyNodeForSystem1, 4, aDouble => aDouble));
			
			var a = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] {
				aLoad1,
				aSetupPressure2,
				0,
				aLoad2,
				aSetupPressure1
			}
			);

			Assert.AreEqual (expectedVector, a);
		}
	}
}

