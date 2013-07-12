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

		public static SpecificationLoader MakeNTurtleSpecificationLoader ()
		{
			return new SpecificationLoader (new TurtleParser ());
		}

		public void LoadFileIntoGraphReraisingParseException (
			String filename, 
			IGraph g)
		{
			this.LoadFileIntoGraph (
				filename, 
				g,
				HandleRdfParseException,
				HandleRdfException
			);
		}

		protected virtual void HandleRdfParseException (RdfParseException ex)
		{
			throw ex;
		}

		protected virtual void HandleRdfException (RdfException ex)
		{
			throw ex;
		}

		public void LoadFileIntoGraph (
			String filename, 
			IGraph g,
			Action<RdfParseException> onRdfParseException,
			Action<RdfException> onRdfException)
		{
			try {
				this.Reader.Load (g, filename);
			} catch (RdfParseException parseEx) {
				onRdfParseException.Invoke (parseEx);
			} catch (RdfException rdfEx) {
				onRdfException.Invoke (rdfEx);
			}
		}

		public object Instantiate (Triple typeSpecification)
		{
			Object instance = null;

			var canProceedToInstantiate = isPredicateNodeForQualifiedTypename (
				typeSpecification.Predicate.AsValuedNode ().AsString ());

			if (canProceedToInstantiate) {

				Type typeToInstantiateFrom = Type.GetType (
					typeSpecification.Object.ToString (), true);

				instance = Activator.CreateInstance (typeToInstantiateFrom);
			} else {
				throw new ArgumentException (
					"The given triple doesn't represent a type instantiation request.");
			}

			return instance;
		}

		protected virtual Boolean isPredicateNodeForQualifiedTypename (
			String aPredicate)
		{
			return  aPredicate.EndsWith ("/qualified-fullname");
		}

		public Dictionary<string, object> InstantiateObjects (
			IGraph g)
		{
			Dictionary<String, Object> objects = 
					new Dictionary<String, Object> ();

			INode rdfTypePredicateMatch = g.CreateUriNode (NamespaceRepository.rdf_type ());

			var triples = g.GetTriplesWithPredicate (rdfTypePredicateMatch);

			foreach (Triple triple in triples) {

				String key = triple.Subject.AsValuedNode ().AsString ();
				if (objects.ContainsKey (key) == false) {

					INode pred = g.CreateUriNode (
						NamespaceRepository.csharp_qualified_fullname ());

					var typeSpecificationsEnumerator = g.GetTriplesWithSubjectPredicate (
						triple.Object, pred).GetEnumerator ();

					if (typeSpecificationsEnumerator.MoveNext () == false) {
						throw new Exception ("Type specification missing");
					}

					var typeSpecification = typeSpecificationsEnumerator.Current;

					if (typeSpecificationsEnumerator.MoveNext ()) {
						throw new Exception ("The type specification is not uniquely determined.");
					}
						
					objects.Add (key, this.Instantiate (typeSpecification));				

				}

			}

			return objects;
		}

	}
}

