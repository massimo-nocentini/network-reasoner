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

		public class MaxMinSetupPressureIntervalFinder : GasNodeVisitor, GasNodeGadgetVisitor
		{
			public AmbientParameters AmbientParameters{ get; set; }

			Double? MaxSeenSetupPressure{ get; set; }

			Double? MinSeenSetupPressure{ get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				// nothing to do since no gadget found for this variant.
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}
			#endregion

			#region GasNodeGadgetVisitor implementation
			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				// nothing to do since the gadget is a load.
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				if (MaxSeenSetupPressure.HasValue) {
					if (aSupplyGadget.SetupPressure > MaxSeenSetupPressure.Value) {
						MaxSeenSetupPressure = aSupplyGadget.SetupPressure;
					}
				} else {
					MaxSeenSetupPressure = aSupplyGadget.SetupPressure;
				}

				if (MinSeenSetupPressure.HasValue) {
					if (aSupplyGadget.SetupPressure < MinSeenSetupPressure.Value) {
						MinSeenSetupPressure = aSupplyGadget.SetupPressure;
					}
				} else {
					MinSeenSetupPressure = aSupplyGadget.SetupPressure;
				}
			}
			#endregion

			public void adimensionalizedMinMax (Action<double, double> continuation)
			{
				// per l'acqua si deve correggere come fatto nelle formule.

				double min = (this.MinSeenSetupPressure.Value / 1000.0 + 
					this.AmbientParameters.AirPressureInBar) /
					this.AmbientParameters.RefPressureInBar;

				double max = (this.MaxSeenSetupPressure.Value / 1000.0 + 
					this.AmbientParameters.AirPressureInBar) /
					this.AmbientParameters.RefPressureInBar;

				continuation.Invoke (Math.Pow (min, 2), Math.Pow (max, 2));
			}
		}

		void smartUnknownsInitialization (
			Random rand, 
			Dictionary<GasNodeAbstract, double> initialUnknowns)
		{
			MaxMinSetupPressureIntervalFinder minMaxIntervalFinder = new MaxMinSetupPressureIntervalFinder {
				AmbientParameters = this.AmbientParameters
			};
			doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (aVertex => aVertex.accept (minMaxIntervalFinder)));
			doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (aVertex => {
				double fixingTerm = .97;
				minMaxIntervalFinder.adimensionalizedMinMax ((min, max) => {
					var maxMinusMin = max - (min * fixingTerm);
					var value = (min * fixingTerm) + (maxMinusMin * rand.NextDouble ());
					initialUnknowns.Add (aVertex, value);
				}
				);
			}
			)
			);
		}

		public Dictionary<GasNodeAbstract, double> makeInitialGuessForUnknowns ()
		{
			var initialUnknowns = new  Dictionary<GasNodeAbstract, double> ();
			var rand = new Random (DateTime.Now.Millisecond);

			doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => {

				double value = 
					(rand.NextDouble()*.1) + 1;

				initialUnknowns.Add (aVertex, value);
			}
			));

//			smartUnknownsInitialization (rand, initialUnknowns);



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

		public static GasNetwork makeFromRemapping (
			Dictionary<GasNodeAbstract, GasNodeAbstract> 
			fixedNodesWithLoadGadgetByOriginalNodes, 
			List<GasEdgeAbstract> values)
		{
			throw new NotImplementedException ();
		}

	}
}

