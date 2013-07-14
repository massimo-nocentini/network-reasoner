using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class ReachabilityValidator
	{
		public Boolean Enabled{ get; set; }

		public Boolean RaiseException{ get; set; }

		public Boolean WriteLog{ get; set; }

		public ReachabilityValidator ()
		{
		}

		public void validate (GasNetwork gasNetwork)
		{
			var visitedFlagsByNode = new Dictionary<GasNodeAbstract, bool> ();
			var neighborhoodByNode = new Dictionary<GasNodeAbstract, List<GasNodeAbstract>> ();

			foreach (GasNodeAbstract node in gasNetwork.Nodes.Values) {
				visitedFlagsByNode.Add (node, false);
				neighborhoodByNode.Add (node, new List<GasNodeAbstract> ());
			}

			foreach (GasEdge edge in gasNetwork.Edges.Values) {
				neighborhoodByNode [edge.StartNode].Add (edge.EndNode);
				neighborhoodByNode [edge.EndNode].Add (edge.StartNode);
			}

			Action<GasNodeAbstract> dfsSearch = null;
			dfsSearch = node => {
				visitedFlagsByNode [node] = true;

				neighborhoodByNode [node].
				FindAll (neighbor => visitedFlagsByNode [neighbor] == false).
				ForEach (neighborNotVisited => dfsSearch.Invoke (neighborNotVisited));
			};

			var enumerator = gasNetwork.Nodes.Values.GetEnumerator ();
			if (enumerator.MoveNext ()) {
			
				dfsSearch.Invoke (enumerator.Current);

				foreach (var pair in visitedFlagsByNode) {
					Boolean isVisited = pair.Value;
					GasNodeAbstract node = pair.Key;
					if (isVisited == false) {
						throw new NetworkNotConnectedException (
						string.Format ("The node ``{0}'' isn't connected to the others.", node.Identifier));
					}
				}

			}

		}
	}
}

