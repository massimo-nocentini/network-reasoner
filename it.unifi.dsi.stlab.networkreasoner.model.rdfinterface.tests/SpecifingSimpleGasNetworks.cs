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

			Assert.AreEqual (3, objectsByUri.Count);
			String loadGadget1Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load1";
			String loadGadget2Key = "http://stlab.dsi.unifi.it/networkreasoner/gadget/load2";
			String supplyGadgetKey = "http://stlab.dsi.unifi.it/networkreasoner/gadget/supply";

			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget1Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetLoad), objectsByUri [loadGadget2Key]);
			Assert.IsInstanceOf (typeof(GasNodeGadgetSupply), objectsByUri [supplyGadgetKey]);

			var loadGadget1 = objectsByUri [loadGadget1Key] as GasNodeGadgetLoad;
			var loadGadget2 = objectsByUri [loadGadget2Key] as GasNodeGadgetLoad;
			var supplyGadget = objectsByUri [supplyGadgetKey] as GasNodeGadgetSupply;

			Assert.AreEqual (463.98, loadGadget1.Load);
			Assert.AreEqual (756.38, loadGadget2.Load);
			Assert.AreEqual (157.34, supplyGadget.SetupPressure);
			Assert.AreEqual (785.23, supplyGadget.MaxQ);
			Assert.AreEqual (100.00, supplyGadget.MinQ);


		}
	}
}

