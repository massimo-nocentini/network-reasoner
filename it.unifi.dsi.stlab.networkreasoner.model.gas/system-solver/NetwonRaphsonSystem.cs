using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetwonRaphsonSystem
	{
		private Dictionary<GasEdgeTopological, Double> Qvector{ get; set; }

		private Dictionary<NodeMatrixConstruction, double> Unknowns { get; set; }

		private List<NodeMatrixConstruction> Nodes{ get; set; }

		private List<GasEdgeTopological> Edges{ get; set; }

		private MatrixComputationDataProvider ComputationDataProvider { get; set; }

		private Dictionary<KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double>
			LittleK { get; set; }

		class NodeForNetwonRaphsonSystem : GasNodeVisitor
		{
			public long Height {
				get;
				set;
			}

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				throw new System.NotImplementedException ();
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				throw new System.NotImplementedException ();
			}
			#endregion
		}

		class EdgeForNetwonRaphsonSystemBuilder : 
			GasEdgeVisitor, GasEdgeGadgetVisitor
		{
			interface EdgeState
			{
				void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector);
			}

			class EdgeStateOn:EdgeState
			{
				#region EdgeState implementation
				public void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector)
				{
					collector.Add (aBuilder.CustomEdgeUnderBuilding);
				}
				#endregion
			}

			class EdgeStateOff:EdgeState
			{
				#region EdgeState implementation
				public void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector)
				{
					// here we simple do not add the edge since it is switched off.
				}
				#endregion
			}

			public Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> customNodesByGeneralNodes{ get; set; }

			public AmbientParameters AmbientParameters{ get; set; }

			public EdgeForNetwonRaphsonSystem CustomEdgeUnderBuilding{ get; set; }

			EdgeState EdgeIsAllowed{ get; set; }

			public EdgeForNetwonRaphsonSystemBuilder ()
			{
				// here we build the edge for the Netwon-Raphson system.
				this.CustomEdgeUnderBuilding = new EdgeForNetwonRaphsonSystem ();
				this.EdgeIsAllowed = new EdgeStateOn ();
			}

			#region GasEdgeVisitor implementation
			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				CustomEdgeUnderBuilding.Diameter = gasEdgePhysical.Diameter;
				CustomEdgeUnderBuilding.Length = gasEdgePhysical.Length;
			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				CustomEdgeUnderBuilding.StartNode = 
					customNodesByGeneralNodes [gasEdgeTopological.StartNode];

				CustomEdgeUnderBuilding.EndNode = 
					customNodesByGeneralNodes [gasEdgeTopological.EndNode];
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				gasEdgeWithGadget.accept (this);

				// here we continue the recursion just for elegance and 
				// foreseeing in the future if we add a gadget for edges that 
				// doesn't switch off the edge.
				gasEdgeWithGadget.Equipped.accept (this);
			}
			#endregion

			#region GasEdgeGadgetVisitor implementation
			public void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
			{
				this.EdgeIsAllowed = new EdgeStateOff ();
			}
			#endregion

			public void buildCustomEdgeFrom (
				GasEdgeAbstract anEdge, List<EdgeForNetwonRaphsonSystem> collector)
			{
				anEdge.accept (this);

				this.EdgeIsAllowed.buildCustomEdgeFor (this, collector);
			}

		}

		// the following is a merely data class with some little behavior
		// attached to it.
		class EdgeForNetwonRaphsonSystem
		{
			public long Length { get; set; }

			public double Diameter { get; set; }

			public NodeForNetwonRaphsonSystem StartNode{ get; set; }

			public NodeForNetwonRaphsonSystem EndNode{ get; set; }

			public AmbientParameters AmbientParameters{ get; set; }

			public double covariantLittleK ()
			{
				return AmbientParameters.Rconstant + weightedHeightsDifference;
			}

			public double controVariantLittleK ()
			{
				return AmbientParameters.Rconstant - weightedHeightsDifference;
			}

			protected virtual double weightedHeightsDifference {
				get {
					var difference = StartNode.Height - EndNode.Height;
					var rate = AmbientParameters.GravitationalAcceleration / AmbientParameters.GasTemperature;
					return rate * difference;
				}
			}


		}

		public void initializeWith (GasNetwork network)
		{
			// before build the nodes

			// now we can build the edges
			List<EdgeForNetwonRaphsonSystem> collector = 
				new List<EdgeForNetwonRaphsonSystem> ();

//			network.doOnEdges (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
//				anEdge => {
//				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
//				aBuilder.AmbientParameters = network.AmbientParameters;
//
//				var customEdge = aBuilder.buildCustomEdgeFrom (anEdge, collector);
//			}
//			)
//			);
		}

		public OneStepMutationResults mutate ()
		{
			var unknownsAtPreviousStep = Unknowns;
			var QvectorAtPreviousStep = Qvector;
			Dictionary<GasEdgeTopological, Double> VvectorAtPreviousStep = null;

			var KvectorAtCurrentStep = computeKvector (
				unknownsAtPreviousStep,
				QvectorAtPreviousStep,
				null,
				null);

			var coefficientsVectorAtCurrentStep = new Dictionary<NodeMatrixConstruction, double> ();

			var matrixAtCurrentStep = new Dictionary<
				KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double> ();

			var dataProviderAtCurrentStep = new MatrixComputationDataProviderDictionaryImplementation (
				LittleK, KvectorAtCurrentStep);

			foreach (var nodeInRow in Nodes) {
				foreach (var nodeInColumn in Nodes) {

					var nodePair = new KeyValuePair<
						NodeMatrixConstruction,NodeMatrixConstruction> (
							nodeInRow, nodeInColumn);

					double value = nodeInRow.matrixValueRespect (
						nodeInColumn, dataProviderAtCurrentStep);

					matrixAtCurrentStep.Add (nodePair, value);

				}

				coefficientsVectorAtCurrentStep [nodeInRow] = nodeInRow.coefficient ();
			}

			var unknownsAtCurrentStep = Solve (
				matrixAtCurrentStep, coefficientsVectorAtCurrentStep);

			var QvectorAtCurrentStep = computeQvector (
				unknownsAtCurrentStep, dataProviderAtCurrentStep);

			Unknowns = unknownsAtCurrentStep;
			Qvector = QvectorAtCurrentStep;

			var result = new OneStepMutationResults ();

			result.Matrix = matrixAtCurrentStep;
			result.Unknowns = unknownsAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;

			return result;
		}

		protected Dictionary<KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double> 
			computeKvector (
			Dictionary<NodeMatrixConstruction, double> unknowns,
			Dictionary<GasEdgeTopological, Double> Qvector, // questo possiamo non passarlo
			Dictionary<GasEdgeTopological, Double> Vvector, // questo possiamo non passarlo
				Dictionary<GasEdgeTopological, Double> Fvector) // di default 0.015, per ogni arco
		{
			foreach (var edge in this.Edges) {
				var f = Fvector [edge];
				var A = 4837.00;
				//var l = edge.Length;
				//unknowns[edge.StartNode.adapterForMatrixConstruction()] - unknowns[edge.EndNode.adapterForMatrixConstruction()]
			}

			return null;
		}

		protected Dictionary<NodeMatrixConstruction, double> Solve (
			Dictionary<KeyValuePair<NodeMatrixConstruction, NodeMatrixConstruction>, double> matrixAtCurrentStep, 
			Dictionary<NodeMatrixConstruction, double> coefficientsVector)
		{
			throw new NotImplementedException ();
		}

		protected Dictionary<GasEdgeTopological, Double> computeQvector (
			Dictionary<NodeMatrixConstruction, double> unknownsAtCurrentStep, 
			MatrixComputationDataProviderDictionaryImplementation dataProviderAtCurrentStep)
		{
			throw new NotImplementedException ();
		}



	}
}

