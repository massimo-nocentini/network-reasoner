using System;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;
using System.Collections.Generic;

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

		public Dictionary<String, GasNodeAbstract> Nodes { get; private set; }

		public Dictionary<String, GasEdgeAbstract> Edges { get; private set; }

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

				double value = unknownInitialization.initialValueFor(aVertex, rand);

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

		public void doOnEdges (NetworkIteratorHandler<GasEdgeAbstract> nodeHandler)
		{
			foreach (var anEdge in this.Edges) {
				nodeHandler.onNodeWithKey (anEdge.Key, anEdge.Value);
				nodeHandler.onRawNode (anEdge.Value);
			}
		}

		public static GasNetwork makeFromRemapping (
			Dictionary<GasNodeAbstract, GasNodeAbstract> 
			fixedNodesWithLoadGadgetByOriginalNodes, 
			List<GasEdgeAbstract> values)
		{
			throw new NotImplementedException ();
		}

	}
}

