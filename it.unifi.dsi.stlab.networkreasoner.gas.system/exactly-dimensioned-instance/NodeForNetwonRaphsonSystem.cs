using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
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

		public void initializeWith (GasNodeAbstract aNode)
		{
			aNode.accept (this);
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
}

