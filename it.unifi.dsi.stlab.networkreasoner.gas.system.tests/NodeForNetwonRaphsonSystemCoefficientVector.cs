using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NodeForNetwonRaphsonSystemCoefficientVector
	{
		GasFormulaVisitor FormulaVisitor{ get; set; }

		public NodeForNetwonRaphsonSystemCoefficientVector ()
		{
			this.FormulaVisitor = new GasFormulaVisitorExactlyDimensioned {
				AmbientParameters = this.valid_initial_ambient_parameters()
			};
		}

		public AmbientParameters valid_initial_ambient_parameters ()
		{
			AmbientParameters parameters = new AmbientParametersGas ();
			parameters.ElementName = "methane";
			parameters.MolWeight = 16.0;
			parameters.ElementTemperatureInKelvin = 288.15;
			parameters.RefPressureInBar = 1.01325;
			parameters.RefTemperatureInKelvin = 288.15;
			parameters.AirPressureInBar = 1.01325;
			parameters.AirTemperatureInKelvin = 288.15;
			parameters.ViscosityInPascalTimesSecond = .0000108;

			return parameters;

		}

		[Test()]
		[ExpectedExceptionAttribute(typeof(NodeForNetwonRaphsonSystem.HeightPropertyMissingException))]
		public void height_property_is_mandatory_for_nodes_for_Newton_Raphson_context ()
		{
			var aSetupPressure = 387.291;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ 
				SetupPressure = aSetupPressure };

			// here we inject an error since we do not give the Height property.
			aSupplier.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);
		}

		[Test()]
		public void building_a_vector_from_one_supplier_only_should_produce_a_vector_with_one_item_equals_to_setup_pressure ()
		{
			var aSetupPressure = 387.291;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ 
				SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological {Height = 1};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (nodeForSystem, 0);
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = 0});

			var expectedVector = new DenseVector (new[] { 1.9103690569894831 });

			Assert.AreEqual (expectedVector, a);
		}

		[Test()]
		public void building_a_vector_from_a_supplier_against_three_other_nodes_should_produce_a_vector_of_length_four ()
		{
			var aSetupPressure = 387.291;
			var defaultForOthers = 0;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ 
				SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological  {Height = 1};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological {Height = 1});

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological {Height = 1});

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological {Height = 1});

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (anotherNode1, 0);
			aList.Add (anotherNode2, 1);
			aList.Add (nodeForSystem, 2);
			aList.Add (anotherNode3, 3);
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = defaultForOthers});

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				1.9103690569894831,
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
			aSupplier1.Equipped = new GasNodeTopological  {Height = 1};

			var nodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem1.initializeWith (aSupplier1);

			var aSupplier2 = new GasNodeWithGadget ();
			aSupplier2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure2 };
			aSupplier2.Equipped = new GasNodeTopological  {Height = 1};

			var nodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem2.initializeWith (aSupplier2);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem1.putYourCoefficientInto (aVector, FormulaVisitor);
			nodeForSystem2.putYourCoefficientInto (aVector, FormulaVisitor);
			
			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological  {Height = 1});

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological  {Height = 1});

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological  {Height = 1});

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (anotherNode1, 0);
			aList.Add (anotherNode2, 1);
			aList.Add (nodeForSystem1, 2);
			aList.Add (anotherNode3, 3);
			aList.Add (nodeForSystem2, 4);
			
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = defaultForOthers});

			var expectedVector = new DenseVector (new[] {
				defaultForOthers,
				defaultForOthers,
				1.9103690569894831,
				defaultForOthers,
				1.0063730987685002
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
			aLoader.Equipped = new GasNodeTopological  {Height = 1};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aLoader);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (nodeForSystem, 0);
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = 0});

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
			aLoader.Equipped = new GasNodeTopological  {Height = 1};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aLoader);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological  {Height = 1});

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological  {Height = 1});

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological  {Height = 1});

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (anotherNode1, 0);
			aList.Add (anotherNode2, 1);
			aList.Add (nodeForSystem, 2);
			aList.Add (anotherNode3, 3);
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = defaultForOthers});

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
			aLoader1.Gadget = new GasNodeGadgetLoad{ Load = aLoad1 };
			aLoader1.Equipped = new GasNodeTopological {Height=1};

			var nodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem1.initializeWith (aLoader1);

			var aLoader2 = new GasNodeWithGadget ();
			aLoader2.Gadget = new GasNodeGadgetLoad{ Load = aLoad2 };
			aLoader2.Equipped = new GasNodeTopological{Height=1};

			var nodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			nodeForSystem2.initializeWith (aLoader2);

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem1.putYourCoefficientInto (aVector, FormulaVisitor);
			nodeForSystem2.putYourCoefficientInto (aVector, FormulaVisitor);
			
			var anotherNode1 = new NodeForNetwonRaphsonSystem ();
			anotherNode1.initializeWith (new GasNodeTopological {Height=1});

			var anotherNode2 = new NodeForNetwonRaphsonSystem ();
			anotherNode2.initializeWith (new GasNodeTopological {Height=1});

			var anotherNode3 = new NodeForNetwonRaphsonSystem ();
			anotherNode3.initializeWith (new GasNodeTopological {Height=1});

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (anotherNode1, 0);
			aList.Add (anotherNode2, 1);
			aList.Add (nodeForSystem1, 2);
			aList.Add (anotherNode3, 3);
			aList.Add (nodeForSystem2, 4);
			
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = defaultForOthers});

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
			nodeForSystem.initializeWith (new GasNodeTopological  {Height = 1});

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			nodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (nodeForSystem, 0);
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = 0});

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
			aSupplier1.Equipped = new GasNodeTopological {Height = 1};

			var supplyNodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			supplyNodeForSystem1.initializeWith (aSupplier1);

			var aSupplier2 = new GasNodeWithGadget ();
			aSupplier2.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure2 };
			aSupplier2.Equipped = new GasNodeTopological {Height = 1};

			var supplyNodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			supplyNodeForSystem2.initializeWith (aSupplier2);

			var aLoad1 = 243.2191;
			var aLoad2 = 233.239;

			var aLoader1 = new GasNodeWithGadget ();
			aLoader1.Gadget = new GasNodeGadgetLoad{ Load = aLoad1 };
			aLoader1.Equipped = new GasNodeTopological {Height = 1};

			var loadNodeForSystem1 = new NodeForNetwonRaphsonSystem ();
			loadNodeForSystem1.initializeWith (aLoader1);

			var aLoader2 = new GasNodeWithGadget ();
			aLoader2.Gadget = new GasNodeGadgetLoad{ Load = aLoad2 };
			aLoader2.Equipped = new GasNodeTopological {Height = 1};

			var loadNodeForSystem2 = new NodeForNetwonRaphsonSystem ();
			loadNodeForSystem2.initializeWith (aLoader2);

			var passiveNodeForSystem = new NodeForNetwonRaphsonSystem ();
			passiveNodeForSystem.initializeWith (new GasNodeTopological {Height = 1});

			var aVector = new Vector<NodeForNetwonRaphsonSystem> ();
			loadNodeForSystem1.putYourCoefficientInto (aVector, FormulaVisitor);
			loadNodeForSystem2.putYourCoefficientInto (aVector, FormulaVisitor);
			supplyNodeForSystem1.putYourCoefficientInto (aVector, FormulaVisitor);
			supplyNodeForSystem2.putYourCoefficientInto (aVector, FormulaVisitor);
			passiveNodeForSystem.putYourCoefficientInto (aVector, FormulaVisitor);

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (loadNodeForSystem1, 0);
			aList.Add (supplyNodeForSystem2, 1);
			aList.Add (passiveNodeForSystem, 2);
			aList.Add (loadNodeForSystem2, 3);
			aList.Add (supplyNodeForSystem1, 4);
			
			var a = aVector.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = 0});

			var expectedVector = new DenseVector (new[] {
				aLoad1,
				1.0063730987685002,
				0,
				aLoad2,
				1.9103690569894831
			}
			);
		
			Assert.AreEqual (expectedVector, a);
		}
	}
}

