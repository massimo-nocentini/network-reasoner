using System;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public	class EdgeStateOff:EdgeState
	{
		#region EdgeState implementation

		public  void putKvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		public void fillAmatrixUsingFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		public void fillJacobianMatrixFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aJacobianMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		public void putQvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		public void putNewFvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> newFvector,
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			GasFormulaVisitor formulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		public void stringRepresentationUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> aVector, 
			Action<string, string> continuation, 
			EdgeForNetwonRaphsonSystem anEdge)
		{

			var edgeRepresentation = anEdge.topologicalStringRepresentation ();

			// since this role makes the edge to which he is attached to switched off
			// we don't care to look for an entry in the given vector.
			continuation.Invoke (edgeRepresentation, "don't care because switched off");
		}

		public void putVelocityValueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> velocityVector, 
			Vector<NodeForNetwonRaphsonSystem> pressures, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			GasFormulaVisitor formulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			// here we don't need to do anything since the edge is switched off.
		}

		#endregion
	}
}

