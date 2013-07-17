using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetwonRaphsonSystem
	{
		private Dictionary<EdgeForNetwonRaphsonSystem, Double> Fvector{ get; set; }

		private Dictionary<NodeForNetwonRaphsonSystem, Double> Unknowns { get; set; }

		private List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		private List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		private MatrixComputationDataProvider ComputationDataProvider { get; set; }

		private Dictionary<KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double>
			LittleK { get; set; }

		class NodeForNetwonRaphsonSystem : GasNodeVisitor, GasNodeGadgetVisitor
		{
			interface NodeRole
			{
				void updateMatrixIfNodeWithSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					NodeForNetwonRaphsonSystem respectToAnotherNode, 
					object matrixToUpdate);

				double coefficient ();
			}

			class NodeRoleSupplier:NodeRole
			{
				public double SetupPressure { get; set; }

				#region NodeRole implementation
				public double coefficient ()
				{
					return SetupPressure;
				}

				public void updateMatrixIfNodeWithSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					NodeForNetwonRaphsonSystem respectToAnotherNode, 
					object matrixToUpdate)
				{
					// update here the matrix
					throw new System.NotImplementedException ();
				}
				#endregion
			}

			class NodeRoleLoader:NodeRole
			{
				public double Load { get; set; }

				#region NodeRole implementation
				public double coefficient ()
				{
					return Load;
				}

				public void updateMatrixIfNodeWithSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					NodeForNetwonRaphsonSystem respectToAnotherNode, 
					object matrixToUpdate)
				{
					// here we do not need to do anything because
					// the receiver node has a load gadget hence
					// its row is already computed and doesn't need
					// to be fixed in this case.
				}
				#endregion
			}

			public long Height { get; set; }

			public NodeRole Role{ get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				this.Height = gasNodeTopological.Height;
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
				var role = new NodeRoleLoader ();
				role.Load = aLoadGadget.Load;
				this.Role = role;
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				var role = new NodeRoleSupplier ();
				role.SetupPressure = aSupplyGadget.SetupPressure;
				this.Role = role;
			}
			#endregion

			public double coefficient ()
			{
				return this.Role.coefficient ();
			}

			public void updateMatrixIfNodeWithSupplyGadget (
				NodeForNetwonRaphsonSystem respectToAnotherNode, object matrixToUpdate)
			{
				this.Role.updateMatrixIfNodeWithSupplyGadgetFor (
					this, respectToAnotherNode, matrixToUpdate);
			}

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
				gasEdgePhysical.Described.accept (this);
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
				this.CustomEdgeUnderBuilding.AmbientParameters = AmbientParameters;

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

			public double coVariantLittleK ()
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
			var FvectorAtPreviousStep = Fvector;

			// Do the following vector is really necessary?
			Dictionary<EdgeForNetwonRaphsonSystem, Double> VvectorAtPreviousStep = null;

			var KvectorAtCurrentStep = computeKvector (
				unknownsAtPreviousStep,
				FvectorAtPreviousStep);

			var coefficientsVectorAtCurrentStep = new Dictionary<NodeForNetwonRaphsonSystem, double> ();

			var matrixAtCurrentStep = new Dictionary<
				KeyValuePair<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem>, Double> ();

			foreach (var anEdge in this.Edges) {
			}

			foreach (var nodeInRow in Nodes) {
				foreach (var nodeInColumn in Nodes) {

					var nodePair = new KeyValuePair<
						NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> (
							nodeInRow, nodeInColumn);

					nodeInRow.updateMatrixIfNodeWithSupplyGadget (
						respectTo: nodeInColumn, 
						matrixToUpdate: matrixAtCurrentStep);
				}

				coefficientsVectorAtCurrentStep [nodeInRow] = nodeInRow.coefficient ();
			}

			var dataProviderAtCurrentStep = new MatrixComputationDataProviderDictionaryImplementation (
				LittleK, KvectorAtCurrentStep);



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

		protected Dictionary<KeyValuePair<NodeForNetwonRaphsonSystem,NodeForNetwonRaphsonSystem>, Double> 
			computeKvector (
			Dictionary<NodeForNetwonRaphsonSystem, double> unknowns,
			Dictionary<EdgeForNetwonRaphsonSystem, Double> Fvector)
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

