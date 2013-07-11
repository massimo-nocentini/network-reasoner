using System;
using VDS.RDF.Parsing;
using VDS.RDF;
using System.IO;
using System.Collections.Generic;
using VDS.RDF.Nodes;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface
{
	public class SpecificationLoader
	{

		private IRdfReader Reader{ get; set; }

		private SpecificationLoader (IRdfReader reader)
		{
			this.Reader = reader;
		}

		public static SpecificationLoader make_nturtle_specification_loader ()
		{
			return new SpecificationLoader (new TurtleParser ());
		}

		public void LoadFile (params String[] filenames)
		{
			foreach (String filename in filenames) {			

				IGraph g = new Graph ();

				Dictionary<String, Object> objects = 
					new Dictionary<String, Object> ();

				//try {
				this.Reader.Load (g, filename);
				//} catch (RdfParseException parseEx) {
				//	//This indicates a parser error e.g unexpected character, premature end of input, invalid syntax etc.
				//	Console.WriteLine ("Parser Error");
				//	Console.WriteLine (parseEx.Message);
				//} catch (RdfException rdfEx) {
				//	//This represents a RDF error e.g. illegal triple for the given syntax, undefined namespace
				//	Console.WriteLine ("RDF Error");
				//	Console.WriteLine (rdfEx.Message);
				//}

				INode rdfTypePredicateMatch = g.CreateUriNode (UriFactory.Create (
					NamespaceRepository.rdf.Append ("type").Value ())
				);

				var triples = g.GetTriplesWithPredicate (rdfTypePredicateMatch);

				foreach (Triple triple in triples) {

					String key = triple.Subject.AsValuedNode ().AsString ();
					if (objects.ContainsKey (key) == false) {

						String typeNameToCreateInstanceFrom = triple.Object.AsValuedNode ().AsString ();
						typeNameToCreateInstanceFrom = typeNameToCreateInstanceFrom.Substring (
							typeNameToCreateInstanceFrom.LastIndexOf ('/') + 1);

						Type typeToCreateInstanceFrom;
						if (TypeMapping.Mapping.TryGetValue (
								typeNameToCreateInstanceFrom, out typeToCreateInstanceFrom)) {

							Object newInstance = Activator.CreateInstance (
								typeToCreateInstanceFrom);						

							objects.Add (key, newInstance);
						}
					}
				}
			}
		}
	}
}

