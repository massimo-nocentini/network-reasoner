using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public interface NodeRole
	{
		void fixMatrixIfYouHaveSupplyGadgetFor (
			NodeForNetwonRaphsonSystem aNode, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix);

		void putYourCoefficientIntoFor (
			NodeForNetwonRaphsonSystem aNode, 
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector);

		GasNodeAbstract substituteNodeBecauseNegativePressureFoundFor (
			NodeForNetwonRaphsonSystem aNode,
			double pressure, 
			GasNodeAbstract correspondingOriginalNode);

	}
}

