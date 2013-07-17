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

		public Dictionary<GasNodeAbstract, double> makeInitialGuessForUnknowns ()
		{
			var initialUnknowns = new  Dictionary<GasNodeAbstract, double> ();
			var rand = new Random (DateTime.Now.Millisecond);

			doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => {
				var value = rand.NextDouble () / 10;
				initialUnknowns.Add (aVertex, value + 1);
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

		public interface NodeHandler<T>
		{
			void onRawNode (T aNode);

			void onNodeWithKey (string aNodeIdentifier, T aNode);
		}

		public abstract class NodeHandlerAbstract<T> : NodeHandler<T>
		{
			#region NodeHandler implementation
			public virtual void onRawNode (T aNode)
			{

			}

			public virtual void onNodeWithKey (
				string aNodeIdentifier, T aNode)
			{

			}
			#endregion
		}

		public class NodeHandlerWithDelegateOnRawNode<T> : NodeHandlerAbstract<T>
		{
			Action<T> aBlock { get; set; }

			public NodeHandlerWithDelegateOnRawNode (Action<T> aBlock)
			{
				this.aBlock = aBlock;
			}

			#region NodeHandler implementation
			public override void onRawNode (T aNode)
			{
				this.aBlock.Invoke (aNode);
			}
			#endregion
			public NodeHandlerWithDelegateOnRawNode (object par)
			{
				throw new NotImplementedException ();
			}

		}

		public class NodeHandlerWithDelegateOnKeyedNode<T> : NodeHandlerAbstract<T>
		{
			Action<String, T> aBlock { get; set; }

			public NodeHandlerWithDelegateOnKeyedNode (
				Action<String, T> aBlock)
			{
				this.aBlock = aBlock;
			}

			public override void onNodeWithKey (
				string aNodeIdentifier, T aNode)
			{
				this.aBlock.Invoke (aNodeIdentifier, aNode);
			}
		}

		public void doOnNodes (NodeHandler<GasNodeAbstract> nodeHandler)
		{
			foreach (var aNode in this.Nodes) {
				nodeHandler.onNodeWithKey (aNode.Key, aNode.Value);
				nodeHandler.onRawNode (aNode.Value);
			}
		}

		public void doOnEdges (NodeHandler<GasEdgeAbstract> nodeHandler)
		{
			foreach (var anEdge in this.Edges) {
				nodeHandler.onNodeWithKey (anEdge.Key, anEdge.Value);
				nodeHandler.onRawNode (anEdge.Value);
			}
		}
	}
}

