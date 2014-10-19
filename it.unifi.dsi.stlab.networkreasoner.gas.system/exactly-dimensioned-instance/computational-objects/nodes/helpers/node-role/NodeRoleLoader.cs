using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{


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
}

