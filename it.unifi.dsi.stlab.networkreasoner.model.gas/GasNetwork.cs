using System;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.extensionmethods;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNetwork
	{
		public class GasParserResultReceiver : ParserResultReceiver
		{
			private GasNetwork Parent { get; set; }

			public GasParserResultReceiver (GasNetwork gasNetwork)
			{
				this.Parent = gasNetwork;
			}

			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.ParserResultReceiver
			public override void receiveResults (
				Dictionary<string, object> objectsByUri)
			{
				foreach (KeyValuePair<String, Object> pair in objectsByUri) {
					if (pair.Value is GasNodeAbstract) {
						this.Parent.Nodes.Add (pair.Key, pair.Value as GasNodeAbstract);
					} else if (pair.Value is GasEdgeAbstract) {
						this.Parent.Edges.Add (pair.Key, pair.Value as GasEdgeAbstract);
					}
				}
			}
			#endregion
		}

		public GasNetwork ()
		{
			this.Nodes = new Dictionary<String, GasNodeAbstract> ();
			this.Edges = new Dictionary<String, GasEdgeAbstract> ();
			this.ParserResultReceiver = new GasParserResultReceiver (this);
		}

		public String Description { get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		public Dictionary<String, GasNodeAbstract> Nodes { get; set; }

		public Dictionary<String, GasEdgeAbstract> Edges { get; set; }

		public ParserResultReceiver ParserResultReceiver { get; set; }

		public ReachabilityValidator ReachabilityValidator{ get; set; }

		public DotRepresentationValidator DotRepresentationValidator { get; set; }

		public Dictionary<GasEdgeAbstract, double> makeInitialGuessForFvector ()
		{
			var initialFvector = new Dictionary<GasEdgeAbstract, double> ();

			doOnEdges (new NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => initialFvector.Add (anEdge, .015))
			);

			return initialFvector;
		}

		public Dictionary<GasNodeAbstract, double> makeInitialGuessForUnknowns (
			UnknownInitialization unknownInitialization)
		{
			var initialUnknowns = new  Dictionary<GasNodeAbstract, double> ();
			var rand = new Random (DateTime.Now.Millisecond);

			doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => {

				double value = unknownInitialization.initialValueFor (aVertex, rand);

				initialUnknowns.Add (aVertex, value);
			}
			)
			);

			return initialUnknowns;
		}

		public void applyValidators ()
		{
			this.ReachabilityValidator.validate (this);
			this.DotRepresentationValidator.validate (this);
		}

		public void doOnNodes (NetworkIteratorHandler<GasNodeAbstract> nodeHandler)
		{
			foreach (var aNode in this.Nodes) {
				nodeHandler.onNodeWithKey (aNode.Key, aNode.Value);
				nodeHandler.onRawNode (aNode.Value);
			}
		}

		public void doOnEdges (NetworkIteratorHandler<GasEdgeAbstract> edgeHandler)
		{
			foreach (var anEdge in this.Edges) {
				edgeHandler.onNodeWithKey (anEdge.Key, anEdge.Value);
				edgeHandler.onRawNode (anEdge.Value);
			}
		}

		public GasNetwork makeFromRemapping (
			List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> fixedNodesWithLoadGadgetByOriginalNodes,
			out List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> edgeSubstitutions)
		{
			GasNetwork newNetwork = new GasNetwork {
				AmbientParameters = this.AmbientParameters
			};

			var substitutedByOriginalsNodesDictionary = 
				fixedNodesWithLoadGadgetByOriginalNodes.SubstitutedByOriginals ();

			this.doOnNodes (new NodeHandlerWithDelegateOnKeyedNode<GasNodeAbstract> (
				(aNodeKey, aNode) => {

				// here isn't necessary to visit the structure of the node since 
				// all the properties are mantained by the node to be substituted,
				// the only thing that change is the gadget.
				var nodeForNewNetwork = substitutedByOriginalsNodesDictionary.ContainsKey (aNode) ?
					substitutedByOriginalsNodesDictionary [aNode] : aNode;

				newNetwork.Nodes.Add (aNodeKey, nodeForNewNetwork);
			}
			)
			);

			var innerEdgeSubstitutions = new List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> ();

			this.doOnEdges (new NodeHandlerWithDelegateOnKeyedNode<GasEdgeAbstract> (
				(anEdgeKey, anEdge) => {

				// creating a new visitor is necessary since every instance of the visitor
				// keep state information about one edge, different at each iteration 
				// of this block.
				SubstituteNodeInsideEdge substituteNodeInsideEdge = 
				new SubstituteNodeInsideEdge{
					NewByOldNodesMapping = substitutedByOriginalsNodesDictionary
				};

				anEdge.accept (substituteNodeInsideEdge);

				var edgeForNewNetwork = substituteNodeInsideEdge.buildEdge ();

				innerEdgeSubstitutions.Add (new ObjectWithSubstitutionInSameType<GasEdgeAbstract>{
					Original = anEdge,
					Substituted = edgeForNewNetwork
				}
				);

				newNetwork.Edges.Add (anEdgeKey, edgeForNewNetwork);
			}
			)
			);

			edgeSubstitutions = innerEdgeSubstitutions;

			return newNetwork;
		}

		class SubstituteNodeInsideEdge : GasEdgeVisitor
		{
			public Dictionary<GasNodeAbstract, GasNodeAbstract> NewByOldNodesMapping;

			#region properties
			GasEdgeGadget Gadget {
				get;
				set;
			}

			double Diameter {
				get;
				set;
			}

			double Length {
				get;
				set;
			}

			double MaxSpeed {
				get;
				set;
			}

			double Roughness {
				get;
				set;
			}

			GasNodeAbstract StartNode{ get; set; }

			GasNodeAbstract EndNode{ get; set; }

			#endregion

			#region GasEdgeVisitor implementation
			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				this.Diameter = gasEdgePhysical.Diameter;
				this.Length = gasEdgePhysical.Length;
				this.MaxSpeed = gasEdgePhysical.MaxSpeed;
				this.Roughness = gasEdgePhysical.Roughness;

				gasEdgePhysical.Described.accept (this);
			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				this.StartNode = applySubstitutionOn (gasEdgeTopological.StartNode);

				this.EndNode = applySubstitutionOn (gasEdgeTopological.EndNode);
			}

			GasNodeAbstract applySubstitutionOn (GasNodeAbstract aNode)
			{
				return NewByOldNodesMapping.ContainsKey (aNode) ?
					NewByOldNodesMapping [aNode] : aNode;
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				this.Gadget = gasEdgeWithGadget.Gadget;
				gasEdgeWithGadget.Equipped.accept (this);
			}
			#endregion

			public GasEdgeAbstract buildEdge ()
			{
				GasEdgeAbstract newEdge = new GasEdgeTopological{
					StartNode = this.StartNode,
					EndNode = this.EndNode
				};

				newEdge = new GasEdgePhysical{
					Described = newEdge,
					Diameter = this.Diameter,
					Length = this.Length,
					MaxSpeed = this.MaxSpeed,
					Roughness = this.Roughness
				};

				if (this.Gadget != null) {
					newEdge = new GasEdgeWithGadget{
						Equipped = newEdge,
						Gadget = this.Gadget
					};
				}

				return newEdge;
			}
		}
	}
}

