using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{


	public class NodeRoleLoader:NodeRole
	{
		class SubstituteGadget : GasNodeVisitor
		{
			public GasNodeGadgetSupply ReplacementGadget {
				get;
				set;
			}

			GasNodeAbstract NewNode {
				get;
				set;
			}

			#region GasNodeVisitor implementation

			public void forNodeWithTopologicalInfo (
				GasNodeTopological gasNodeTopological)
			{
				this.NewNode = gasNodeTopological;
			}

			public void forNodeWithGadget (
				GasNodeWithGadget gasNodeWithGadget)
			{
				// first we go down the tower in order to collect every piece of information
				gasNodeWithGadget.Equipped.accept (this);

				// now we can wrap the collected result with
				// the new information.
				this.NewNode = new GasNodeWithGadget {
					Equipped = this.NewNode,
					Gadget = this.ReplacementGadget
				};
			}

			public void forNodeAntecedentInPressureReduction (
				GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
			{
				throw new NotSupportedException ("An antecedent node in a pressure regulation relation" +
					" cannot have a pressure lesser than conseguent's pressure, by definition of" +
					" pressure regulator gadget.");

				//gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);

				// after the recursion unwinds we can top the obtained result with
				// the informations that this node is an antecedent in
				// a pressure regulation relation. But, since the node
				// will become a supply node with 0 pressure, is it correct
				// to proceed with this configuration, where the antecedent in a 
				// pressure regulation has a pressure lesser than the pressure
				// at the conseguent?
				// TODO: ask Fabio that the previous reasoning is correct or not.
			}

			#endregion

			/// <summary>
			/// Syntactic sugar method, just visit the given node and build another
			/// node tower with the given gadget supplied.
			/// </summary>
			/// <param name="correspondingOriginalNode">Corresponding original node.</param>
			public GasNodeAbstract on (GasNodeAbstract correspondingOriginalNode)
			{
				correspondingOriginalNode.accept (this);
				return this.NewNode;
			}

		}

		public double Load { get; set; }

		#region NodeRole implementation

		public virtual void putYourCoefficientIntoFor (
			NodeForNetwonRaphsonSystem aNode, 
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
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
			return new SubstituteGadget {
				ReplacementGadget = new GasNodeGadgetSupply {
					SetupPressure = 0d
				}
			}.on (correspondingOriginalNode);

		}

		#endregion

	}
}

