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
					if (pair.Value is GasNode) {
						this.Parent.Nodes.Add (pair.Key, pair.Value as GasNode);
					} else if (pair.Value is GasEdge) {
						this.Parent.Edges.Add (pair.Key, pair.Value as GasEdge);
					}
				}
			}
			#endregion
		}

		public GasNetwork ()
		{
			this.Nodes = new Dictionary<object, GasNode> ();
			this.Edges = new Dictionary<object, GasEdge> ();
			this.ParserResultReceiver = new GasParserResultReceiver (this);
		}

		public String Description { get; set; }

		public Dictionary<Object, GasNode> Nodes { get; private set; }

		public Dictionary<Object, GasEdge> Edges { get; private set; }

		public ParserResultReceiver ParserResultReceiver { get; set; }

		public ReachabilityValidator ReachabilityValidator{ get; set; }

		public Dictionary<GasEdge, double> makeInitialGuessForQvector ()
		{
			throw new NotImplementedException ();
		}

		public Dictionary<NodeMatrixConstruction, double> makeInitialGuessForUnknowns ()
		{
			throw new NotImplementedException ();
		}

	}
}

