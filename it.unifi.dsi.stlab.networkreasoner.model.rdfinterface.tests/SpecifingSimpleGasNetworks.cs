using System;
using NUnit.Framework;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.tests
{
	[TestFixture()]
	public class SpecifingSimpleGasNetworks
	{
		[Test()]
		public void instantiate_an_object_from_triple_type_specification ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			Graph g = new Graph ();
			g.NamespaceMap.AddNamespace ("obj", UriFactory.Create ("http://example.org/object/"));
			g.NamespaceMap.AddNamespace ("type", UriFactory.Create ("http://example.org/type/"));
			g.NamespaceMap.AddNamespace ("csharp", UriFactory.Create ("http://example.org/csharp/"));
			g.NamespaceMap.AddNamespace ("rdf", UriFactory.Create ("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));

			IUriNode type = g.CreateUriNode ("type:aType");
			IUriNode csharpEffectiveTypePred = g.CreateUriNode ("csharp:qualified-fullname");
			ILiteralNode csharpEffectiveType = g.CreateLiteralNode (
				"it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.tests.DummyTypeForInstantiation, it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.tests");

			Triple typeSpecification = new Triple (
				type, csharpEffectiveTypePred, csharpEffectiveType);

			Object instance = loader.Instantiate (typeSpecification);

			Assert.IsInstanceOf (typeof(DummyTypeForInstantiation), instance);
		}

		[Test()]
		public void load_specification_into_a_graph ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-with-gadgets-without-edges.nt";
			IGraph g = new Graph ();

			loader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Assert.AreEqual (19, g.Triples.Count);
		}

		[Test()]
		public void instantiate_objects_inside_a_graph_with_only_one_node_specification ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-checking-object-instantiation.nt";
			IGraph g = new Graph ();

			loader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Dictionary<String, Object> objectsByUri = loader.InstantiateObjects (g);

			Assert.AreEqual (3, objectsByUri.Count);
			String singleObjectKey = "http://stlab.dsi.unifi.it/networkreasoner/node/single-node";
			String loadGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load";
			String supplyGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/supply";
			Assert.IsTrue (objectsByUri.ContainsKey (singleObjectKey));
			Assert.IsTrue (objectsByUri.ContainsKey (loadGadgetKey));
			Assert.IsTrue (objectsByUri.ContainsKey (supplyGadgetKey));
			Assert.IsInstanceOf (typeof(DummyTypeForInstantiation), objectsByUri [singleObjectKey]);
			Assert.IsInstanceOf (typeof(AnotherDummyObject), objectsByUri [loadGadgetKey]);
			Assert.IsInstanceOf (typeof(MoreDummyObject), objectsByUri [supplyGadgetKey]);
		}

		[Test()]
		public void set_literal_properties ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-checking-literal-properties.nt";
			IGraph g = new Graph ();

			loader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Dictionary<String, Object> objectsByUri = loader.InstantiateObjects (g);

			loader.setPropertiesOnInstances (objectsByUri, g);

			Assert.AreEqual (4, objectsByUri.Count);
			String loadGadget1Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load1";
			String loadGadget2Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load2";
			String supplyGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/supply";
			String nodeKey = "http://stlab.dsi.unifi.it/networkreasoner/node/node";

			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget1Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget2Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetSupply), objectsByUri [supplyGadgetKey]);
			Assert.IsInstanceOf (typeof(GasNodeTopological), objectsByUri [nodeKey]);

			var loadGadget1 = objectsByUri [loadGadget1Key] as GasNodeGadgetLoad;
			var loadGadget2 = objectsByUri [loadGadget2Key] as GasNodeGadgetLoad;
			var supplyGadget = objectsByUri [supplyGadgetKey] as GasNodeGadgetSupply;
			var node = objectsByUri [nodeKey] as GasNodeTopological;

			Assert.AreEqual (463.98, loadGadget1.Load);
			Assert.AreEqual (756.38, loadGadget2.Load);
			Assert.AreEqual (157.34, supplyGadget.SetupPressure);
			Assert.AreEqual (785.23, supplyGadget.MaxQ);
			Assert.AreEqual (100.00, supplyGadget.MinQ);

			Assert.AreEqual ("single node", node.Identifier);
			Assert.AreEqual (35, node.Height);
			Assert.AreEqual ("this is the very first node that we build with our system.", node.Comment);


		}

		[Test()]
		public void load_vertices_equipped_with_gadgets ()
		{
			var specLoader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-loading-equipped-vertices.nt";
			IGraph g = new Graph ();

			specLoader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Dictionary<String, Object> objectsByUri = specLoader.InstantiateObjects (g);

			specLoader.setPropertiesOnInstances (objectsByUri, g);

			Assert.AreEqual (7, objectsByUri.Count);

			String loadGadget1Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load1";
			String loadGadget2Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load2";
			String supplyGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/supply";
			String nodeKey = "http://stlab.dsi.unifi.it/networkreasoner/node/node";
			String nodeToBeEquippedWithSupplyGadgetKey = 
				"http://stlab.dsi.unifi.it/networkreasoner/node/node-to-be-equipped-with-supply-gadget";
			String loaderKey = "http://stlab.dsi.unifi.it/networkreasoner/node/loader";
			String supplierKey = "http://stlab.dsi.unifi.it/networkreasoner/node/supplier";


			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget1Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget2Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetSupply), objectsByUri [supplyGadgetKey]);
			Assert.IsInstanceOf (typeof(GasNodeTopological), objectsByUri [nodeKey]);
			Assert.IsInstanceOf (typeof(GasNodeTopological), objectsByUri [nodeToBeEquippedWithSupplyGadgetKey]);
			Assert.IsInstanceOf (typeof(GasNodeWithGadget), objectsByUri [loaderKey]);
			Assert.IsInstanceOf (typeof(GasNodeWithGadget), objectsByUri [supplierKey]);

			var loadGadget1 = objectsByUri [loadGadget1Key] as GasNodeGadgetLoad;
			var loadGadget2 = objectsByUri [loadGadget2Key] as GasNodeGadgetLoad;
			var supplyGadget = objectsByUri [supplyGadgetKey] as GasNodeGadgetSupply;
			var node = objectsByUri [nodeKey] as GasNodeTopological;
			var nodeToBeEquippedWithSupplyGadget = objectsByUri [nodeToBeEquippedWithSupplyGadgetKey] as GasNodeTopological;
			var loader = objectsByUri [loaderKey] as GasNodeWithGadget;
			var supplier = objectsByUri [supplierKey] as GasNodeWithGadget;

			Assert.AreEqual (463.98, loadGadget1.Load);
			Assert.AreEqual (756.38, loadGadget2.Load);
			Assert.AreEqual (157.34, supplyGadget.SetupPressure);
			Assert.AreEqual (785.23, supplyGadget.MaxQ);
			Assert.AreEqual (100.00, supplyGadget.MinQ);

			Assert.AreEqual ("I'll be a loader", node.Identifier);
			Assert.AreEqual (35, node.Height);
			Assert.AreEqual ("this is the very first node that we build with our system.", node.Comment);

			Assert.AreEqual ("I'll be a supplier", nodeToBeEquippedWithSupplyGadget.Identifier);
			Assert.AreEqual (46, nodeToBeEquippedWithSupplyGadget.Height);
			Assert.AreEqual ("", nodeToBeEquippedWithSupplyGadget.Comment);

			Assert.IsNotNull (loader.Equipped);
			Assert.IsNotNull (loader.Gadget);
			Assert.AreSame (node, loader.Equipped);
			Assert.AreSame (loadGadget1, loader.Gadget);

			Assert.IsNotNull (supplier.Equipped);
			Assert.IsNotNull (supplier.Gadget);
			Assert.AreSame (nodeToBeEquippedWithSupplyGadget, supplier.Equipped);
			Assert.AreSame (supplyGadget, supplier.Gadget);

		}

		[Test()]
		public void load_vertices_equipped_with_gadgets_substituting_decorator_objects ()
		{
			var specLoader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-loading-equipped-vertices-with-decoration-substitution.nt";
			IGraph g = new Graph ();
			Dictionary<String, Object> objectsByUri;
			specLoader.ReifySpecification (filenameToParse, g, out objectsByUri);

			Assert.AreEqual (5, objectsByUri.Count);

			String loadGadget1Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load1";
			String loadGadget2Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load2";
			String supplyGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/supply";
			String nodeKey = "http://stlab.dsi.unifi.it/networkreasoner/node/node";
			String nodeToBeEquippedWithSupplyGadgetKey = 
				"http://stlab.dsi.unifi.it/networkreasoner/node/node-to-be-equipped-with-supply-gadget";
			String loaderKey = "http://stlab.dsi.unifi.it/networkreasoner/node/loader";
			String supplierKey = "http://stlab.dsi.unifi.it/networkreasoner/node/supplier";

			Assert.IsFalse (objectsByUri.ContainsKey (nodeKey));
			Assert.IsFalse (objectsByUri.ContainsKey (nodeToBeEquippedWithSupplyGadgetKey));

			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget1Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget2Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetSupply), objectsByUri [supplyGadgetKey]);
			Assert.IsInstanceOf (typeof(GasNodeWithGadget), objectsByUri [loaderKey]);
			Assert.IsInstanceOf (typeof(GasNodeWithGadget), objectsByUri [supplierKey]);

			var loadGadget1 = objectsByUri [loadGadget1Key] as GasNodeGadgetLoad;
			var loadGadget2 = objectsByUri [loadGadget2Key] as GasNodeGadgetLoad;
			var supplyGadget = objectsByUri [supplyGadgetKey] as GasNodeGadgetSupply;
			var loader = objectsByUri [loaderKey] as GasNodeWithGadget;
			var supplier = objectsByUri [supplierKey] as GasNodeWithGadget;

			Assert.IsNotNull (loader.Equipped);
			Assert.IsNotNull (loader.Gadget);
			Assert.IsInstanceOf (typeof(GasNodeTopological), loader.Equipped);

			Assert.IsNotNull (supplier.Equipped);
			Assert.IsNotNull (supplier.Gadget);
			Assert.IsInstanceOf (typeof(GasNodeTopological), supplier.Equipped);

			var node = loader.Equipped as GasNodeTopological;
			var nodeToBeEquippedWithSupplyGadget = supplier.Equipped as GasNodeTopological;

			Assert.AreEqual (463.98, loadGadget1.Load);
			Assert.AreEqual (756.38, loadGadget2.Load);
			Assert.AreEqual (157.34, supplyGadget.SetupPressure);
			Assert.AreEqual (785.23, supplyGadget.MaxQ);
			Assert.AreEqual (100.00, supplyGadget.MinQ);

			Assert.AreEqual ("I'll be a loader", node.Identifier);
			Assert.AreEqual (35, node.Height);
			Assert.AreEqual ("this is the very first node that we build with our system.", node.Comment);

			Assert.AreEqual ("I'll be a supplier", nodeToBeEquippedWithSupplyGadget.Identifier);
			Assert.AreEqual (46, nodeToBeEquippedWithSupplyGadget.Height);
			Assert.AreEqual ("", nodeToBeEquippedWithSupplyGadget.Comment);
		}

		[Test()]
		public void build_edges_and_set_object_properties ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-checking-edge-and-uri-objects.nt";
			IGraph g = new Graph ();
			Dictionary<String, Object> objectsByUri;

			loader.ReifySpecification (filenameToParse, g, out objectsByUri);

			Assert.AreEqual (8, objectsByUri.Count);

			String nodeAKey = "http://stlab.dsi.unifi.it/networkreasoner/network/nodeA";
			String nodeBKey = "http://stlab.dsi.unifi.it/networkreasoner/network/nodeB";
			String nodeCKey = "http://stlab.dsi.unifi.it/networkreasoner/network/nodeC";
			String nodeDKey = "http://stlab.dsi.unifi.it/networkreasoner/network/nodeD";

			String edgeADKey = "http://stlab.dsi.unifi.it/networkreasoner/network/edgeAD-physical";
			String edgeABKey = "http://stlab.dsi.unifi.it/networkreasoner/network/edgeAB-physical";
			String edgeBCKey = "http://stlab.dsi.unifi.it/networkreasoner/network/edgeBC-physical";
			String edgeDBKey = "http://stlab.dsi.unifi.it/networkreasoner/network/edgeDB-physical";

			// we do not check the types for nodes since we have already
			// a test case for them.
			Assert.IsInstanceOf (typeof(GasEdgePhysical), objectsByUri [edgeADKey]);
			Assert.IsInstanceOf (typeof(GasEdgePhysical), objectsByUri [edgeABKey]);
			Assert.IsInstanceOf (typeof(GasEdgePhysical), objectsByUri [edgeBCKey]);
			Assert.IsInstanceOf (typeof(GasEdgePhysical), objectsByUri [edgeDBKey]);

			var nodeA = objectsByUri [nodeAKey] as GasNodeTopological;
			var nodeB = objectsByUri [nodeBKey] as GasNodeTopological;
			var nodeC = objectsByUri [nodeCKey] as GasNodeTopological;
			var nodeD = objectsByUri [nodeDKey] as GasNodeTopological;

			var edgeAD = objectsByUri [edgeADKey] as GasEdgePhysical;
			var edgeAB = objectsByUri [edgeABKey] as GasEdgePhysical;
			var edgeBC = objectsByUri [edgeBCKey] as GasEdgePhysical;
			var edgeDB = objectsByUri [edgeDBKey] as GasEdgePhysical;

			Assert.IsInstanceOf (typeof(GasEdgeTopological), edgeAD.Described);
			Assert.IsInstanceOf (typeof(GasEdgeTopological), edgeAB.Described);
			Assert.IsInstanceOf (typeof(GasEdgeTopological), edgeBC.Described);
			Assert.IsInstanceOf (typeof(GasEdgeTopological), edgeDB.Described);

			var edgeADTopological = edgeAD.Described as GasEdgeTopological;
			var edgeABTopological = edgeAB.Described as GasEdgeTopological;
			var edgeBCTopological = edgeBC.Described as GasEdgeTopological;
			var edgeDBTopological = edgeDB.Described as GasEdgeTopological;

			Assert.AreEqual (45.968, edgeAD.Diameter);
			Assert.AreSame (nodeD, edgeADTopological.EndNode);
			Assert.AreEqual (1500, edgeAD.Length);
			Assert.AreEqual (12.35, edgeAD.MaxSpeed);
			Assert.AreEqual (1.7, edgeAD.Roughness);
			Assert.AreSame (nodeA, edgeADTopological.StartNode);

			Assert.AreEqual (4.8, edgeAB.Diameter);
			Assert.AreSame (nodeB, edgeABTopological.EndNode);
			Assert.AreEqual (150, edgeAB.Length);
			Assert.AreEqual (4, edgeAB.MaxSpeed);
			Assert.AreEqual (4.7, edgeAB.Roughness);
			Assert.AreSame (nodeA, edgeABTopological.StartNode);

			Assert.AreEqual (5.968, edgeBC.Diameter);
			Assert.AreSame (nodeC, edgeBCTopological.EndNode);
			Assert.AreEqual (3400, edgeBC.Length);
			Assert.AreEqual (2.35, edgeBC.MaxSpeed);
			Assert.AreEqual (17, edgeBC.Roughness);
			Assert.AreSame (nodeB, edgeBCTopological.StartNode);

			Assert.AreEqual (459.68, edgeDB.Diameter);
			Assert.AreSame (nodeB, edgeDBTopological.EndNode);
			Assert.AreEqual (15000, edgeDB.Length);
			Assert.AreEqual (123, edgeDB.MaxSpeed);
			Assert.AreEqual (17, edgeDB.Roughness);
			Assert.AreSame (nodeD, edgeDBTopological.StartNode);

		}

		[Test()]
		public void load_main_network_object ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-loading-the-main-network-object.nt";

			IGraph g = new Graph ();

			loader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Dictionary<String, Object> objectsByUri = loader.InstantiateObjects (g);

			loader.setPropertiesOnInstances (objectsByUri, g);

			var mainNetwork = loader.FindMainNetwork (objectsByUri, g);

			Assert.IsInstanceOf (typeof(GasNetwork), mainNetwork);

			var gasNetwork = mainNetwork as GasNetwork;
			Assert.AreEqual ("This network represent a gas network, actually an empty network.", gasNetwork.Description);
			CollectionAssert.IsEmpty (gasNetwork.Edges);
			CollectionAssert.IsEmpty (gasNetwork.Nodes);
		}

		[Test()]
		public void check_if_parser_receiver_is_correctly_set_in_main_network_object ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-loading-the-main-network-object-with-result-receiver.nt";

			IGraph g = new Graph ();

			loader.LoadFileIntoGraphReraisingParseException (filenameToParse, g);

			Dictionary<String, Object> objectsByUri = loader.InstantiateObjects (g);

			loader.setPropertiesOnInstances (objectsByUri, g);

			var mainNetwork = loader.FindMainNetwork (objectsByUri, g);

			var parserResultReceiver = loader.GetParserResultReceiverFrom (objectsByUri, g);

			Assert.IsInstanceOf (typeof(GasParserResultReceiver), 
			                     parserResultReceiver);

			var gasNetwork = mainNetwork as GasNetwork;

			Assert.AreSame (gasNetwork.ParserResultReceiver, parserResultReceiver);
		}

		[Test()]
		public void load_a_complete_network_with_a_main_object_defined_in_specification ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/gas/specification-for-loading-a-network-with-a-main-object-defined-in-it.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network);
			Assert.IsInstanceOf (typeof(GasNetwork), network);
			Assert.IsNotNull (network.ParserResultReceiver);
			Assert.AreEqual ("This network represent a gas network", network.Description);

			Assert.AreEqual (4, network.Nodes.Count);
			Assert.AreEqual (4, network.Edges.Count);
		}
	}
}

