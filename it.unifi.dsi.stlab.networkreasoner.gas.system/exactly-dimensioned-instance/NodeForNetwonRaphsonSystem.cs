using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NodeForNetwonRaphsonSystem : GasNodeVisitor, GasNodeGadgetVisitor
	{
		public class NodeWithoutGadgetFound:Exception
		{
		}

		public interface NodeRole
		{
			void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, Double> aMatrix);

			void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem, Double> aVector);
		}

		public class NodeRoleSupplier:NodeRole
		{
			public double SetupPressure { get; set; }

				#region NodeRole implementation
			public void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem, Double> aVector)
			{
				aVector.atPut (aNode, SetupPressure);
			}

			public void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aRowNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix)
			{
				aMatrix.doOnRowOf (aRowNode, (aColumnNode, cumulate) => aRowNode.Equals (aColumnNode) ? 1 : 0);
			}
				#endregion

		}

		public class NodeRoleLoader:NodeRole
		{
			public double Load { get; set; }

				#region NodeRole implementation
			public void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem, Double> aVector)
			{
				aVector.atPut (aNode, Load);
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

		public NodeRole Role{ get; set; }

		public void initializeWith (GasNodeAbstract aNode)
		{
			aNode.accept (this);

			if (this.Role == null) {
				throw new NodeWithoutGadgetFound ();
			}
		}

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
			this.Role = new NodeRoleLoader{ Load = aLoadGadget.Load};
		}

		public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
		{
			this.Role = new NodeRoleSupplier { SetupPressure = aSupplyGadget.SetupPressure};
		}
			#endregion

		public void putYourCoefficientInto (Vector<NodeForNetwonRaphsonSystem, Double> aVector)
		{
			this.Role.putYourCoefficientIntoFor (this, aVector);
		}

		public void fixMatrixIfYouHaveSupplyGadget (
				Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double>  aMatrix)
		{
			this.Role.fixMatrixIfYouHaveSupplyGadgetFor (this, aMatrix);
		}

	}
}

