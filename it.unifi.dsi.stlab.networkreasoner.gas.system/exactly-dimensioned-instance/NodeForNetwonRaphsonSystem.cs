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

		public interface AntecedentInPressureRegulation
		{

		}

		public class IsAntecedentInPressureRegulation : AntecedentInPressureRegulation
		{
			public GasNodeAbstract Regulator{ get; set; }
		}

		public class IsNotAntecedentInPressureRegulation : AntecedentInPressureRegulation
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
				public GasNodeGadgetSupply ReplacementSupplyGadget {
					get;
					set;
				}

				public GasNodeAbstract NewNode {
					get;
					private set;
				}

				#region GasNodeVisitor implementation

				public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
				{
					this.NewNode = gasNodeTopological;
				}

				public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
				{
					// first we go down the tower in order to collect every piece of information
					gasNodeWithGadget.Equipped.accept (this);

					// now we can wrap the collected result with
					// the new information.
					this.NewNode = new GasNodeWithGadget {
						Equipped = this.NewNode,
						Gadget = this.ReplacementSupplyGadget
					};
				}

				public void forNodeAntecedentInPressureReduction (
					GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
				{
					gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);

					// after the recursion unwind we can to the obtained result with
					// the informations that this node is an antecedent in
					// a pressure regulation relation. But, since the node
					// will become a supply node, do we really need 
					// to re-apply this layer of information on the new node?
					// TODO: ask Fabio that the previous reasoning is correct or not.
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
				var nodeMaker = new NewNodeMaker {
					ReplacementSupplyGadget = new GasNodeGadgetSupply {
						SetupPressure = 0d
					}
				};

				correspondingOriginalNode.accept (nodeMaker);

				return nodeMaker.NewNode;
			}

			#endregion

		}

		public class NodeRolePassive:NodeRoleLoader
		{
			public NodeRolePassive ()
			{
				this.Load = 0d;
			}

			public override void putYourCoefficientIntoFor (
				NodeForNetwonRaphsonSystem aNode, 
				Vector<NodeForNetwonRaphsonSystem> aVector,
				GasFormulaVisitor aFormulaVisitor)
			{
				aVector.atPut (aNode, 0d);
			}
		}

		public long Height { get; set; }

		public NodeRole Role{ get; set; }

		public AntecedentInPressureRegulation RoleInPressureRegulation{ get; set; }

		public void initializeWith (GasNodeAbstract aNode)
		{
			aNode.accept (this);

			if (this.Role == null) {
				this.Role = new NodeRolePassive ();
			}

			if (this.RoleInPressureRegulation == null) {
				this.RoleInPressureRegulation = new IsNotAntecedentInPressureRegulation ();
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

		public void forNodeAntecedentInPressureReduction (
			GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
		{
			this.RoleInPressureRegulation = new IsAntecedentInPressureRegulation {
				Regulator = gasNodeAntecedentInPressureRegulator.RegulatorNode
			};
			gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
		}

		#endregion

		#region GasNodeGadgetVisitor implementation

		public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
		{
			this.Role = new NodeRoleLoader { 
				Load = aLoadGadget.Load
			};
		}

		public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
		{
			this.Role = new NodeRoleSupplier { 
				SetupPressureInMillibar = aSupplyGadget.SetupPressure
			};
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

		public double relativeDimensionalPressureOf (
			double absolutePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			var formula = new RelativePressureFromAdimensionalPressureFormulaForNodes ();
			formula.NodeHeight = this.Height;
			formula.AdimensionalPressure = absolutePressure;

			return formula.accept (aFormulaVisitor);
		}

		public double absoluteDimensionalPressureOf (
			double absolutePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			var formula = new AbsolutePressureFromAdimensionalPressureFormulaForNodes ();
			formula.AdimensionalPressure = absolutePressure;

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

