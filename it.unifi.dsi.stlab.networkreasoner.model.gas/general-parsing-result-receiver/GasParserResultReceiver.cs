using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
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
}

