using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.extension_methods;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNetwork
	{
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
			List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> nodesSubstitutions,
			out List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> edgeSubstitutions)
		{
			GasNetwork newNetwork = new GasNetwork {
				AmbientParameters = this.AmbientParameters
			};

			var substitutedByOriginalsNodesDictionary = 
				nodesSubstitutions.SubstitutedByOriginals ();

			this.doOnNodes (new NodeHandlerWithDelegateOnKeyedNode<GasNodeAbstract> (
				makeNodeSubstitutionAction (newNetwork, substitutedByOriginalsNodesDictionary)));

			var innerEdgeSubstitutions = new List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> ();

			this.doOnEdges (new NodeHandlerWithDelegateOnKeyedNode<GasEdgeAbstract> (
				makeEdgeSubstitutionAction (newNetwork, substitutedByOriginalsNodesDictionary, innerEdgeSubstitutions)));

			edgeSubstitutions = innerEdgeSubstitutions;

			return newNetwork;
		}

		#region Virtual methods about nodes and edges substitution actions

		protected virtual Action<String, GasNodeAbstract> makeNodeSubstitutionAction (
			GasNetwork newNetwork, 
			Dictionary<GasNodeAbstract, GasNodeAbstract> substitutedByOriginalsNodesDictionary)
		{
			return (aNodeKey, aNode) => {
				newNetwork.Nodes.Add (aNodeKey, substitutedByOriginalsNodesDictionary.MapIfPossible (aNode));
			};
		}

		protected virtual Action<String, GasEdgeAbstract> makeEdgeSubstitutionAction (
			GasNetwork newNetwork, 
			Dictionary<GasNodeAbstract, GasNodeAbstract> substitutedByOriginalsNodesDictionary, 
			List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> innerEdgeSubstitutions)
		{
			return (anEdgeKey, anEdge) => {

				var edgeForNewNetwork = new SubstituteNodeInsideEdge {
					NewByOldNodesMapping = substitutedByOriginalsNodesDictionary
				}.buildEdgeFrom (anEdge);

				innerEdgeSubstitutions.Add (new ObjectWithSubstitutionInSameType<GasEdgeAbstract> {
					Original = anEdge,
					Substituted = edgeForNewNetwork
				});

				newNetwork.Edges.Add (anEdgeKey, edgeForNewNetwork);
			};
		}

		#endregion

		class SubstituteNodeInsideEdge : GasEdgeVisitor
		{
			public Dictionary<GasNodeAbstract, GasNodeAbstract> NewByOldNodesMapping;

			GasEdgeAbstract BuildingEdge { get; set; }

			#region GasEdgeVisitor implementation

			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				// before go down in the recursive edge tower...
				gasEdgePhysical.Described.accept (this);

				// ...after we can wrap this piece of information
				this.BuildingEdge = new GasEdgePhysical { 
					Described = this.BuildingEdge,
					Diameter = gasEdgePhysical.Diameter,
					Length = gasEdgePhysical.Length,
					MaxSpeed = gasEdgePhysical.MaxSpeed,
					Roughness = gasEdgePhysical.Roughness
				};

			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				// In the base of recursion we simply perform substitution
				this.BuildingEdge = new GasEdgeTopological {
					Identifier = gasEdgeTopological.Identifier,
					StartNode = this.NewByOldNodesMapping.MapIfPossible (gasEdgeTopological.StartNode),
					EndNode = this.NewByOldNodesMapping.MapIfPossible (gasEdgeTopological.EndNode)
				};
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				gasEdgeWithGadget.Equipped.accept (this);

				this.BuildingEdge = new GasEdgeWithGadget { 
					Equipped = this.BuildingEdge,
					Gadget = gasEdgeWithGadget.Gadget
				};
			}

			#endregion

			public GasEdgeAbstract buildEdgeFrom (GasEdgeAbstract anEdge)
			{
				// before visit the given edge
				anEdge.accept (this);

				// then return the refinement
				return this.BuildingEdge;
			}
		}
	}
}

