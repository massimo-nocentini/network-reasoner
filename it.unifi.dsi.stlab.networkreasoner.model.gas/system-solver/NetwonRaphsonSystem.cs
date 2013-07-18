using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetwonRaphsonSystem
	{
		Vector<EdgeForNetwonRaphsonSystem, Double> Fvector{ get; set; }

		Vector<NodeForNetwonRaphsonSystem, Double> UnknownVector { get; set; }

		List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		AmbientParameters AmbientParameters { get; set; }

		internal class NodeForNetwonRaphsonSystem : GasNodeVisitor, GasNodeGadgetVisitor
		{
			interface NodeRole
			{
				void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix);

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

				public void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aRowNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix)
				{
					aMatrix.doOnRowOf (aRowNode, 
					                   (aColumnNode, cumulate) => aRowNode.Equals (aColumnNode) ? 1 : 0);
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

				public void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix)
				{
					// here we do not need to do anything because
					// the receiver node has a load gadget hence
					// its row is already computed and doesn't need
					// to be fixed in this case.
				}
				#endregion
			}

			public long Height { get; set; }

			NodeRole Role{ get; set; }

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

			public void fixMatrixIfYouHaveSupplyGadget (
				Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double>  aMatrix)
			{
				this.Role.fixMatrixIfYouHaveSupplyGadgetFor (this, aMatrix);
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
		internal class EdgeForNetwonRaphsonSystem
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
			this.AmbientParameters = network.AmbientParameters;

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
			var unknownVectorAtPreviousStep = UnknownVector;

			var FvectorAtPreviousStep = Fvector;

			var KvectorAtCurrentStep = computeKvector (
				unknownVectorAtPreviousStep,
				FvectorAtPreviousStep);

			var coefficientsVectorAtCurrentStep = new Vector<NodeForNetwonRaphsonSystem, Double> ();

			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, Double> matrixAtCurrentStep =
				computeMatrix (KvectorAtCurrentStep);

			foreach (var aNode in Nodes) {

				aNode.fixMatrixIfYouHaveSupplyGadget (matrixAtCurrentStep);

				coefficientsVectorAtCurrentStep.atPut (aNode, aNode.coefficient ());
			}

			var result = new OneStepMutationResults ();

			// the following assignments need revision
			result.Matrix = matrixAtCurrentStep;
			result.Unknowns = unknownVectorAtPreviousStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			//result.Qvector = QvectorAtCurrentStep;

			return result;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> 
			computeMatrix (Vector<EdgeForNetwonRaphsonSystem, double> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> ();

			foreach (var anEdge in this.Edges) {
			
				var coVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.coVariantLittleK ();
				var controVariant = kvectorAtCurrentStep.valueAt (anEdge) * (-1) * anEdge.controVariantLittleK ();

				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.StartNode, cumulate => -coVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.EndNode, cumulate => controVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.StartNode, cumulate => coVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.EndNode, cumulate => -controVariant + cumulate, 0);
			}

			return aMatrix;
		}

		Vector<EdgeForNetwonRaphsonSystem, Double> computeKvector (
			Vector<NodeForNetwonRaphsonSystem, Double> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem, Double> Fvector)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem, double> ();

			foreach (var anEdge in this.Edges) {

				var f = Fvector.valueAt (anEdge);
				var A = this.AmbientParameters.Aconstant / Math.Pow (anEdge.Diameter, 5);
				var unknownForStartNode = unknownVector.valueAt (anEdge.StartNode);
				var unknownForEndNode = unknownVector.valueAt (anEdge.EndNode);
				
				var weightedHeightsDifference = 
					anEdge.coVariantLittleK () * unknownForStartNode - 
					anEdge.controVariantLittleK () * unknownForEndNode;

				var K = 1 / Math.Sqrt (f * A * anEdge.Length * weightedHeightsDifference);

				Kvector.atPut (anEdge, K);
			}

			return Kvector;
		}


	}
}

