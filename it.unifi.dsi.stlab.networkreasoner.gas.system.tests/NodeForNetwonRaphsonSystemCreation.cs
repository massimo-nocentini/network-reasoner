using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NodeForNetwonRaphsonSystemCreation
	{
		[Test()]
		public void creating_a_node_for_system_from_one_with_supply_gadget ()
		{

			var aSetupPressure = 387.291;
			var aComment = "a simple comment";
			var anHeight = 4938;
			var anIdentifier = "an simple identifier";

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological { 
				Comment= aComment,
				Height = anHeight,
				Identifier = anIdentifier};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			Assert.AreEqual (anHeight, nodeForSystem.Height);
			Assert.IsInstanceOf (typeof(NodeForNetwonRaphsonSystem.NodeRoleSupplier), nodeForSystem.Role);

			// since we know from the previous assert that the cast is safe.
			var supplierRole = nodeForSystem.Role as NodeForNetwonRaphsonSystem.NodeRoleSupplier;
			Assert.AreEqual (aSetupPressure, supplierRole.SetupPressure);
		}

		[Test()]
		public void creating_a_node_for_system_from_one_with_load_gadget ()
		{

			var aLoad = 45.2;
			var aComment = "a simple comment";
			var anHeight = 4938;
			var anIdentifier = "an simple identifier";

			var aLoader = new GasNodeWithGadget ();
			aLoader.Gadget = new GasNodeGadgetLoad{ Load = aLoad };
			aLoader.Equipped = new GasNodeTopological { 
				Comment= aComment,
				Height = anHeight,
				Identifier = anIdentifier};

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aLoader);

			Assert.AreEqual (anHeight, nodeForSystem.Height);
			Assert.IsInstanceOf (typeof(NodeForNetwonRaphsonSystem.NodeRoleLoader), nodeForSystem.Role);

			// since we know from the previous assert that the cast is safe.
			var loaderRole = nodeForSystem.Role as NodeForNetwonRaphsonSystem.NodeRoleLoader;
			Assert.AreEqual (aLoad, loaderRole.Load);
		}

		[Test()]
		public void creating_a_node_for_system_from_one_without_gadget_should_produce_a_passive_node_with_load_zero()
		{

			var aComment = "a simple comment";
			var anHeight = 4938;
			var anIdentifier = "an simple identifier";

			var aTopologicalNode = new GasNodeTopological { 
				Comment= aComment,
				Height = anHeight,
				Identifier = anIdentifier};			

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aTopologicalNode);

			Assert.AreEqual (anHeight, nodeForSystem.Height);
			Assert.IsInstanceOf (typeof(NodeForNetwonRaphsonSystem.NodeRolePassive), nodeForSystem.Role);

			// since we know from the previous assert that the cast is safe.
			var passiveRole = nodeForSystem.Role as NodeForNetwonRaphsonSystem.NodeRolePassive;
			Assert.AreEqual (0, passiveRole.Load);
		}
	}
}

