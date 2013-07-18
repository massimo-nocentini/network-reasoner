using System;
using NUnit.Framework;
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.tests
{
	[TestFixture()]
	public class LearningTest
	{
		[Test()]
		public void just_make_a_graph_with_dummy_uri ()
		{
			IGraph g = new Graph ();
			g.BaseUri = new Uri ("http://example.org/");
		}

		[Test()]
		public void an_empty_graph_should_not_have_any_knowledge ()
		{
			IGraph g = new Graph ();
			g.BaseUri = new Uri ("http://example.org/");

			Assert.AreEqual (0, g.Triples.Count);
		}

		[Test()]
		public void create_a_node ()
		{
			//We need a graph first
			IGraph g = new Graph ();
			g.BaseUri = UriFactory.Create ("http://example.org/");

			//Create a URI Node that refers to some specific URI
			IUriNode dotNetRDF = g.CreateUriNode (UriFactory.Create ("http://www.dotnetrdf.org"));

			System.Console.WriteLine (dotNetRDF.ToString ());

		}

		[Test()]
		public void create_a_triple ()
		{
			//Need a Graph first
			IGraph g = new Graph ();

			//Create some Nodes
			IUriNode dotNetRDF = g.CreateUriNode (UriFactory.Create ("http://www.dotnetrdf.org"));
			IUriNode createdBy = g.CreateUriNode (UriFactory.Create ("http://example.org/createdBy"));
			ILiteralNode robVesse = g.CreateLiteralNode ("Rob Vesse");

			//Assert this Triple
			Triple t = new Triple (dotNetRDF, createdBy, robVesse);
			g.Assert (t);
		}

		[Test()]
		public void create_a_very_simple_graph_and_write_it_to_files ()
		{
		
			//Fill in the code shown on this page here to build your hello world application
			Graph g = new Graph ();

			IUriNode dotNetRDF = g.CreateUriNode (UriFactory.Create ("http://www.dotnetrdf.org"));
			IUriNode says = g.CreateUriNode (UriFactory.Create ("http://example.org/says"));
			ILiteralNode helloWorld = g.CreateLiteralNode ("Hello World");
			ILiteralNode bonjourMonde = g.CreateLiteralNode ("Bonjour tout le Monde", "fr");

			ILiteralNode value = g.CreateLiteralNode ("12345", 
			                                          new Uri (XmlSpecsHelper.XmlSchemaDataTypeDouble));

			g.Assert (new Triple (dotNetRDF, says, helloWorld));
			g.Assert (new Triple (dotNetRDF, says, bonjourMonde));
			g.Assert (new Triple (dotNetRDF, says, value));

			foreach (Triple t in g.Triples) {
				Console.WriteLine (t.ToString ());
			}

			NTriplesWriter ntwriter = new NTriplesWriter ();
			ntwriter.Save (g, "HelloWorld-with-inline-namespace.nt");

			RdfXmlWriter rdfxmlwriter = new RdfXmlWriter ();
			rdfxmlwriter.Save (g, "HelloWorld-with-inline-namespace.rdf");
		}

		[Test()]
		public void create_a_very_simple_graph_with_namespace_map_and_write_it_to_files ()
		{
		
			//Fill in the code shown on this page here to build your hello world application
			Graph g = new Graph ();
			g.NamespaceMap.AddNamespace ("ex", UriFactory.Create ("http://example.org/namespace/"));
			g.NamespaceMap.AddNamespace ("main", UriFactory.Create ("http://www.dotnetrdf.org/namespace/"));

			IUriNode dotNetRDF = g.CreateUriNode ("main:main-object");
			IUriNode says = g.CreateUriNode ("ex:says");
			ILiteralNode helloWorld = g.CreateLiteralNode ("Hello World");
			ILiteralNode bonjourMonde = g.CreateLiteralNode ("Bonjour tout le Monde", "fr");

			g.Assert (new Triple (dotNetRDF, says, helloWorld));
			g.Assert (new Triple (dotNetRDF, says, bonjourMonde));

			foreach (Triple t in g.Triples) {
				Console.WriteLine (t.ToString ());
			}

			NTriplesWriter ntwriter = new NTriplesWriter ();
			var helloWorldmappednamespacent_nturtle_syntax = "HelloWorld-mapped-namespace.nt";
			ntwriter.Save (g, helloWorldmappednamespacent_nturtle_syntax);

			RdfXmlWriter rdfxmlwriter = new RdfXmlWriter ();
			var helloWorldmappednamespacerdf_rdfxml_syntax = "HelloWorld-mapped-namespace.rdf";
			rdfxmlwriter.Save (g, helloWorldmappednamespacerdf_rdfxml_syntax);

			Assert.IsTrue (File.Exists (helloWorldmappednamespacent_nturtle_syntax));
			Assert.IsTrue (File.Exists (helloWorldmappednamespacerdf_rdfxml_syntax));
		}

		[Test()]
		public void read_a_sample_file_with_prefix_namespaces_and_shortcut_syntax_from_w3c_example ()
		{
			IGraph g = new Graph ();

			TurtleParser ttlparser = new TurtleParser ();

			//Load using a Filename
			var filename_to_parse = "../../nturtle-specifications/file-to-load-from-w3c-nturtle-page.nt";
			ttlparser.Load (g, filename_to_parse);

			Assert.AreEqual (7, g.Triples.Count);


		}

	}
}

