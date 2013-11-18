using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using System.Collections.Generic;
using System.Linq;
using it.unifi.dsi.stlab.utilities.object_with_substitution;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NodeForNetwonRaphsonSystem :
		AbstractItemForNetwonRaphsonSystem, GasNodeVisitor, GasNodeGadgetVisitor
	{
		public class HeightPropertyMissingException : Exception
		{
		}

		public interface NodeRole
		{
			void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix);

			void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem> aVector,
				GasFormulaVisitor aFormulaVisitor);

			GasNodeAbstract substituteNodeBecauseNegativePressureFoundFor (
				NodeForNetwonRaphsonSystem aNode,
				double pressure, 
				GasNodeAbstract correspondingOriginalNode);

		}

		public class NodeRoleSupplier:NodeRole
		{
			public double SetupPressureInMillibar { get; set; }

				#region NodeRole implementation
			public void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem> aVector,
				GasFormulaVisitor aFormulaVisitor)
			{				
				var formula = new CoefficientFormulaForNodeWithSupplyGadget ();
				formula.NodeHeight = aNode.Height;
				formula.GadgetSetupPressureInMillibar = this.SetupPressureInMillibar;

				double Hsetup = formula.accept (aFormulaVisitor);
				aVector.atPut (aNode, Hsetup);
			}

			public void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aRowNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix)
			{
				aMatrix.updateRow (aRowNode, 
				                   (aColumnNode, cumulate) => 
				                   		aRowNode.Equals (aColumnNode) ? 1 : 0
				);
			}

			public GasNodeAbstract substituteNodeBecauseNegativePressureFoundFor (
				NodeForNetwonRaphsonSystem aNode, 
				double pressure, 
				GasNodeAbstract correspondingOriginalNode)
			{
				throw new Exception ("It is impossible to perform a substitution " +
					"during negative loads checking: this is a role for a node with " +
					"supply gadget, hence a contradiction occurs if we found a " +
					"negative load for this node."
				);
			}
			#endregion


		}

		public class NodeRoleLoader:NodeRole
		{
			public class NewNodeMaker : GasNodeVisitor
			{
				GasNodeGadgetSupply ReplacementSupplyGadget {
					get;
					set;
				}

				public GasNodeWithGadget GasNodeWithGadget {
					get;
					set;
				}

				public NewNodeMaker (GasNodeGadgetSupply gasNodeGadgetSupply)
				{
					this.ReplacementSupplyGadget = gasNodeGadgetSupply;
				}

				#region GasNodeVisitor implementation
				public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
				{
					throw new Exception ("We cannot visit looking for substitution " +
						"if we do not find a node decorated with a gadget before this variant."
					);
				}

				public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
				{
					this.GasNodeWithGadget = new GasNodeWithGadget{
						Equipped = gasNodeWithGadget.Equipped,
						Gadget = this.ReplacementSupplyGadget
					};
				}
				#endregion

			}

			public double Load { get; set; }

			#region NodeRole implementation
			public virtual void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem> aVector,
				GasFormulaVisitor aFormulaVisitor)
			{
				aVector.atPut (aNode, Load);
			}

			public virtual void fixMatrixIfYouHaveSupplyGadgetFor (
					NodeForNetwonRaphsonSystem aNode, 
					Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix)
			{
				// here we do not need to do anything because
				// the receiver node has a load gadget hence
				// its row is already computed and doesn't need
				// to be fixed in this case.
			}

			public GasNodeAbstract substituteNodeBecauseNegativePressureFoundFor (
				NodeForNetwonRaphsonSystem aNode, 
				double pressure, 
				GasNodeAbstract correspondingOriginalNode)
			{
				var nodeMaker = new NewNodeMaker (
					new GasNodeGadgetSupply{
						SetupPressure = 0
				}
				);

				correspondingOriginalNode.accept (nodeMaker);

				return nodeMaker.GasNodeWithGadget;
			}
			#endregion

		}

		public class NodeRolePassive:NodeRoleLoader
		{
			public NodeRolePassive ()
			{
				this.Load = 0;
			}

			public override void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem> aVector,
				GasFormulaVisitor aFormulaVisitor)
			{
				aVector.atPut (aNode, 0);
			}
		}

		public long Height { get; set; }

		public string Identifier {
			get;
			set;
		}

		public NodeRole Role{ get; set; }

		public void initializeWith (GasNodeAbstract aNode)
		{
			aNode.accept (this);

			if (this.Role == null) {
				this.Role = new NodeRolePassive ();
			}
		}

			#region GasNodeVisitor implementation
		public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
		{
			if (gasNodeTopological.Height.HasValue == false) {
				throw new HeightPropertyMissingException ();
			}
			this.Height = gasNodeTopological.Height.Value;
			this.Identifier = gasNodeTopological.Identifier;
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
			this.Role = new NodeRoleLoader{ 
				Load = aLoadGadget.Load};
		}

		public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
		{
			this.Role = new NodeRoleSupplier { 
				SetupPressureInMillibar = aSupplyGadget.SetupPressure};
		}
			#endregion

		public void putYourCoefficientInto (
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor)
		{
			this.Role.putYourCoefficientIntoFor (
				this, aVector, aFormulaVisitor);
		}

		public void fixMatrixIfYouHaveSupplyGadget (
				Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem>  aMatrix)
		{
			this.Role.fixMatrixIfYouHaveSupplyGadgetFor (this, aMatrix);
		}

		public double dimensionalPressureOf (
			double absolutePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			var formula = new RelativePressureFromAbsolutePressureFormulaForNodes ();
			formula.NodeHeight = this.Height;
			formula.AbsolutePressure = absolutePressure;

			return formula.accept (aFormulaVisitor);
		}

		public GasNodeAbstract substituteNodeBecauseNegativePressureFound (
			double pressure, GasNodeAbstract originalNode)
		{
			return Role.substituteNodeBecauseNegativePressureFoundFor (
				this, pressure, originalNode);
		}

	}
}

