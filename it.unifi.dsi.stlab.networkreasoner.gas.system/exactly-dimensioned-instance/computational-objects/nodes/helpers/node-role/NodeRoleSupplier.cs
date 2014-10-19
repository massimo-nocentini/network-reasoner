using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public class NodeRoleSupplier:NodeRole
	{
		public double SetupPressureInMillibar { get; set; }

		#region NodeRole implementation

		public void putYourCoefficientIntoFor (
			NodeForNetwonRaphsonSystem aNode, 
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
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
			aMatrix.updateRow (
				aRowNode, 
				(aColumnNode, cumulate) => aRowNode.Equals (aColumnNode) ? 1d : 0d
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
}

