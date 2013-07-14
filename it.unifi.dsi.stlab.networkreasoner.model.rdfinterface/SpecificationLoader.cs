using System;
using VDS.RDF.Parsing;
using VDS.RDF;
using System.IO;
using System.Collections.Generic;
using VDS.RDF.Nodes;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.exceptions;
using System.Reflection;

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

					var enumerableOfTypeSpecifications = g.GetTriplesWithSubjectPredicate (
						triple.Object, pred);						

					var typeSpecification = enumerableOfTypeSpecifications.
						FetchExactlyOneThrowingExceptionsIfErrorsOccurs ();				
						
					objects.Add (key, this.Instantiate (typeSpecification));				

				}

			}

			return objects;
		}

		public class A : INodeExtensionMethods.NodeTypeDispatchingThrowExceptionForEachCase
		{

			Action forLiteralNode { get; set; }

			Action forUriNode { get; set; }

			public A (Action forLiteralNode, Action forUriNode, Exception ex):base(ex)
			{

				this.forLiteralNode = forLiteralNode;
				this.forUriNode = forUriNode;
			}

			public override void literalNode (ILiteralNode iLiteralNode)
			{
				forLiteralNode.Invoke ();
			}

			public override void uriNode (IUriNode iUriNode)
			{
				forUriNode.Invoke ();
			}
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

					triple.Object.DispatchOnNodeType (new A (
						forLiteralNode: () => {

						Object value = this.ValueOfLiteralNode (triple.Object as LiteralNode);
						setProperty (propertyToSet, instance, value);
					}, forUriNode: () => {
						Object alreadyExistingObject = this.ValueOfUriNode (
							triple.Object as UriNode, objectsByUri);
						setProperty (propertyToSet, instance, alreadyExistingObject);
					}, ex: new ShouldNotImplementException ())
					);


				
				}
			}
		}

		protected virtual void setProperty (
			PropertyInfo property, Object instance, Object value)
		{
			property.SetValue (instance, value, null);
		}

		private Object ValueOfUriNode (UriNode node, 
		                               Dictionary<string, object> objectsByUri)
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

		public void Load (
			string filename, ParserResultReceiver parserResultReceiver)
		{
			IGraph g = new Graph ();

			this.LoadFileIntoGraphReraisingParseException (filename, g);

			Dictionary<String, Object> objectsByUri = this.InstantiateObjects (g);

			this.setPropertiesOnInstances (objectsByUri, g);

			parserResultReceiver.receiveResults (objectsByUri);
		}

		public object FindMainNetwork (Dictionary<string, object> objectsByUri, IGraph g)
		{
			var triples = g.GetTriplesWithPredicate (
				NamespaceRepository.tag_main_network ());

			var mainNetworkTripleWithTag = triples.
				FetchExactlyOneThrowingExceptionsIfErrorsOccurs ();

			String key = mainNetworkTripleWithTag.Subject.AsValuedNode ().AsString ();

			return objectsByUri [key];
		}

		public ParserResultReceiver GetParserResultReceiverFrom (
			Dictionary<string, object> objectsByUri, IGraph g)
		{
			var triples = g.GetTriplesWithPredicate (
				NamespaceRepository.tag_parser_result_receiver_property_name ());

			var mainNetworkTripleWithTag = triples.
				FetchExactlyOneThrowingExceptionsIfErrorsOccurs ();

			String mainNetworkKey = mainNetworkTripleWithTag.Subject.AsValuedNode ().AsString ();
			String propertyName = mainNetworkTripleWithTag.Object.ToString ();

			Object mainNetwork = objectsByUri [mainNetworkKey];
			var parserResultReceiver = mainNetwork.GetType ().GetProperty (
				propertyName).GetValue (mainNetwork, null);

			return parserResultReceiver as ParserResultReceiver;
		}





	}
}

