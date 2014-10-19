using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public	interface EdgeState
	{
		void putKvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge);

		void fillAmatrixUsingFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge);

		void fillJacobianMatrixFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aJacobianMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge);

		void putQvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge);

		void putNewFvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> newFvector,
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			GasFormulaVisitor formulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge);

		void stringRepresentationUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> aVector, 
			Action<string, string> continuation, 
			EdgeForNetwonRaphsonSystem anEdge);

		void putVelocityValueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> velocityVector, 
			Vector<NodeForNetwonRaphsonSystem> pressures, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			GasFormulaVisitor formulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge);
	}
}

