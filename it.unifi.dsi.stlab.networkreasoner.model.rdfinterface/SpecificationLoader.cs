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

			Action<ILiteralNode> forLiteralNode { get; set; }

			Action<IUriNode> forUriNode { get; set; }

			public A (Action<ILiteralNode> forLiteralNode, 
			          Action<IUriNode> forUriNode, 
			          Exception ex):base(ex)
			{

				this.forLiteralNode = forLiteralNode;
				this.forUriNode = forUriNode;
			}

			public override void literalNode (ILiteralNode iLiteralNode)
			{
				forLiteralNode.Invoke (iLiteralNode);
			}

			public override void uriNode (IUriNode iUriNode)
			{
				forUriNode.Invoke (iUriNode);
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
						forLiteralNode: aLiteralNode => setProperty (
							propertyToSet, instance, ValueOfLiteralNode (aLiteralNode))
					, forUriNode: aUriNode => {
						Object alreadyExistingObject = this.ValueOfUriNode (
							aUriNode, objectsByUri);
						setProperty (propertyToSet, instance, alreadyExistingObject);
					}, ex: new ShouldNotImplementException ())
					);


				
				}
			}
		}

		protected virtual void setProperty (
			PropertyInfo property, Object instance, Object value)
		{
			try {
				property.SetValue (instance, value, null);
			} catch (Exception e) {
				throw e;
			}
		}

		protected virtual Object ValueOfUriNode (IUriNode node, 
		                               Dictionary<string, object> objectsByUri)
		{
			Func<Uri, Object> mapper = uri => {

				return objectsByUri [uri.AbsoluteUri];
			};

			return node.GetValue (mapper);
		}

		protected virtual Object ValueOfLiteralNode (ILiteralNode node)
		{
			return node.GetValue ();
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

		public void Load (
			string filename, ParserResultReceiver parserResultReceiver)
		{
			IGraph g = new Graph ();
			Dictionary<String, Object> objectsByUri = null;

			ReifySpecification (filename, g, out objectsByUri);

			sendResultTo (parserResultReceiver, objectsByUri);
		}

		public T Load <T> (string filename) where T : class
		{
			IGraph g = new Graph ();
			Dictionary<String, Object> objectsByUri = null;

			ReifySpecification (filename, g, out objectsByUri);

			T mainNetwork = this.FindMainNetwork (objectsByUri, g)as T;

			var parserResultReceiver = this.GetParserResultReceiverFrom (objectsByUri, g);

			sendResultTo (parserResultReceiver, objectsByUri);

			return mainNetwork;
		}

		protected virtual void sendResultTo (
			ParserResultReceiver parserResultReceiver, 
			Dictionary<String, Object> parserResult)
		{
			parserResultReceiver.receiveResults (parserResult);
		}

		public void ReifySpecification (string filename, 
		                IGraph g, 
		                out Dictionary<string, object> objectsByUri)
		{
			this.LoadFileIntoGraphReraisingParseException (filename, g);
			objectsByUri = this.InstantiateObjects (g);
			this.setPropertiesOnInstances (objectsByUri, g);
			this.removeDefinitionsAfterParsing (objectsByUri, g);
		}

		public void removeDefinitionsAfterParsing (
			Dictionary<string, object> objectsByUri, IGraph g)
		{

			var triples = g.GetTriplesWithPredicate (
				NamespaceRepository.tag_delete_after_parsing ());

			foreach (Triple triple in triples) {


				var handler = new A (aLiteralNode => {
					Boolean reallyDelete;
					if (Boolean.TryParse (aLiteralNode.Value, out reallyDelete)) {
						if (reallyDelete) {
							String objKeyToDelete = triple.Subject.AsValuedNode ().AsString ();
							objectsByUri.Remove (objKeyToDelete);
							//				g.Retract (triple);
						}

					}

				}, aUriNode => {}, null);

				triple.Object.DispatchOnNodeType (handler);



			}


		}
	}
}

