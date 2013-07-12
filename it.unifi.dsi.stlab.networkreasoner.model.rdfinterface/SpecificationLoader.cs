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

		public void setPropertiesOnInstances (
			Dictionary<string, object> objectsByUri, IGraph g)
		{
			foreach (Triple triple in g.Triples) {
				String predicateAsString = triple.Predicate.AsValuedNode ().AsString ();
				if (predicateAsString.Contains ("/property/")) {

					String propertyName = predicateAsString.Substring (
						predicateAsString.LastIndexOf ("/") + 1);

					String subjectAsString = triple.Subject.AsValuedNode ().AsString ();
					Object instance = objectsByUri [subjectAsString];
					var propertyToSet = instance.GetType ().GetProperty (propertyName);

					if (triple.Object.NodeType == NodeType.Literal) {

						Object value = this.ValueOfLiteralNode (triple.Object as LiteralNode);
						propertyToSet.SetValue (instance, value, null);

					} else if (triple.Object.NodeType == NodeType.Uri) {
						Object alreadyExistingObject = this.ValueOfUriNode (
							triple.Object as UriNode, objectsByUri);
						propertyToSet.SetValue (instance, alreadyExistingObject, null);
					}
				
				}
			}
		}

		private Object ValueOfUriNode (UriNode node, Dictionary<string, object> objectsByUri)
		{
			String uri = node.Uri.AbsoluteUri;
			return  objectsByUri [uri];
		}

		private Object ValueOfLiteralNode (LiteralNode node)
		{
			if (node.DataType == null) {
				return node.AsValuedNode ().AsString ();
			}

			String nodeTypeAsString = node.DataType.Fragment.Replace ("#", "");
			if (nodeTypeAsString.Equals ("double")) {
				return node.AsValuedNode ().AsDouble ();
			} else if (nodeTypeAsString.Equals ("integer")) {
				return node.AsValuedNode ().AsInteger ();
			}

			throw new ArgumentException (string.Format (
				"the given literal node has a type {0} which isn't used in this application.",
				nodeTypeAsString)
			);
		}	


	}
}

