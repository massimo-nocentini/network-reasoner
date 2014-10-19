using System;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public	class EdgeStateOn:EdgeState
	{
		#region EdgeState implementation

		public  void putKvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge)
		{
			KvalueFormula formula = new KvalueFormula ();

			formula.EdgeFvalue = Fvector.valueAt (anEdge);
			formula.EdgeDiameterInMillimeters = anEdge.DiameterInMillimeters;
			formula.UnknownForEdgeStartNode = unknownVector.valueAt (anEdge.StartNode);
			formula.UnknownForEdgeEndNode = unknownVector.valueAt (anEdge.EndNode);
			formula.EdgeCovariantLittleK = anEdge.coVariantLittleK (aFormulaVisitor);
			formula.EdgeControVariantLittleK = anEdge.controVariantLittleK (aFormulaVisitor);
			formula.EdgeLength = anEdge.Length;

			var K = formula.accept (aFormulaVisitor);
			Kvector.atPut (anEdge, K);
		}

		public void fillAmatrixUsingFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			AmatrixQuadrupletFormulaForSwitchedOnEdges formula =
				new AmatrixQuadrupletFormulaForSwitchedOnEdges ();

			formula.EdgeKvalue = kvectorAtCurrentStep.valueAt (anEdge);
			formula.EdgeCovariantLittleK = anEdge.coVariantLittleK (aFormulaVisitor);
			formula.EdgeControVariantLittleK = anEdge.controVariantLittleK (aFormulaVisitor);

			AmatrixQuadruplet quadruplet = formula.accept (aFormulaVisitor);

			aMatrix.atRowAtColumnPut (
				anEdge.StartNode, 
				anEdge.StartNode, 
				quadruplet.StartNodeStartNodeUpdater, 
				quadruplet.StartNodeStartNodeInitialValue);

			aMatrix.atRowAtColumnPut (
				anEdge.StartNode, 
				anEdge.EndNode, 
				quadruplet.StartNodeEndNodeUpdater, 
				quadruplet.StartNodeEndNodeInitialValue);

			aMatrix.atRowAtColumnPut (
				anEdge.EndNode, 
				anEdge.StartNode, 
				quadruplet.EndNodeStartNodeUpdater, 
				quadruplet.EndNodeStartNodeInitialValue);

			aMatrix.atRowAtColumnPut (
				anEdge.EndNode, 
				anEdge.EndNode, 
				quadruplet.EndNodeEndNodeUpdater, 
				quadruplet.EndNodeEndNodeInitialValue);
		}

		public void fillJacobianMatrixFor (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aJacobianMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			JacobianMatrixQuadrupletFormulaForSwitchedOnEdges formula =
				new JacobianMatrixQuadrupletFormulaForSwitchedOnEdges ();

			formula.EdgeKvalue = kvectorAtCurrentStep.valueAt (anEdge);
			formula.EdgeCovariantLittleK = anEdge.coVariantLittleK (aFormulaVisitor);
			formula.EdgeControVariantLittleK = anEdge.controVariantLittleK (aFormulaVisitor);

			AmatrixQuadruplet quadruplet = formula.accept (aFormulaVisitor);

			aJacobianMatrix.atRowAtColumnPut (
				anEdge.StartNode, 
				anEdge.StartNode, 
				quadruplet.StartNodeStartNodeUpdater, 
				quadruplet.StartNodeStartNodeInitialValue);

			aJacobianMatrix.atRowAtColumnPut (
				anEdge.StartNode, 
				anEdge.EndNode, 
				quadruplet.StartNodeEndNodeUpdater, 
				quadruplet.StartNodeEndNodeInitialValue);

			aJacobianMatrix.atRowAtColumnPut (
				anEdge.EndNode, 
				anEdge.StartNode, 
				quadruplet.EndNodeStartNodeUpdater, 
				quadruplet.EndNodeStartNodeInitialValue);

			aJacobianMatrix.atRowAtColumnPut (
				anEdge.EndNode, 
				anEdge.EndNode, 
				quadruplet.EndNodeEndNodeUpdater, 
				quadruplet.EndNodeEndNodeInitialValue);
		}

		public void putQvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			GasFormulaVisitor aFormulaVisitor,
			EdgeForNetwonRaphsonSystem anEdge)
		{
			QvalueFormula formula = new QvalueFormula ();
			formula.EdgeCovariantLittleK = anEdge.coVariantLittleK (aFormulaVisitor);
			formula.EdgeControVariantLittleK = anEdge.controVariantLittleK (aFormulaVisitor);
			formula.UnknownForEdgeStartNode = unknownVector.valueAt (anEdge.StartNode);
			formula.UnknownForEdgeEndNode = unknownVector.valueAt (anEdge.EndNode);
			formula.EdgeKvalue = Kvector.valueAt (anEdge);

			var Qvalue = formula.accept (aFormulaVisitor);

			Qvector.atPut (anEdge, Qvalue);
		}

		public void putNewFvalueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> newFvector,
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			GasFormulaVisitor formulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			FvalueFormula formula = new FvalueFormula ();

			formula.EdgeQvalue = Qvector.valueAt (anEdge);
			formula.EdgeDiameterInMillimeters = anEdge.DiameterInMillimeters;
			formula.EdgeRoughnessInMicron = anEdge.RoughnessInMicron;
			formula.EdgeFvalue = Fvector.valueAt (anEdge);

			var Fvalue = formula.accept (formulaVisitor);

			newFvector.atPut (anEdge, Fvalue);


		}

		public void stringRepresentationUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> aVector, 
			Action<string, string> continuation, 
			EdgeForNetwonRaphsonSystem anEdge)
		{

			var edgeRepresentation = anEdge.topologicalStringRepresentation ();

			// here we assume that the given vector contains an index for the
			// given edge since this edge is switched on and should 
			// be involved in the computation
			continuation.Invoke (edgeRepresentation, aVector.valueAt (anEdge).ToString ());
		}

		public void putVelocityValueIntoUsingFor (
			Vector<EdgeForNetwonRaphsonSystem> velocityVector, 
			Vector<NodeForNetwonRaphsonSystem> pressures, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			GasFormulaVisitor formulaVisitor, 
			EdgeForNetwonRaphsonSystem anEdge)
		{
			VelocityValueFormula formula = new VelocityValueFormula ();
			formula.AbsolutePressureOfEndNode = pressures.valueAt (anEdge.EndNode);
			formula.AbsolutePressureOfStartNode = pressures.valueAt (anEdge.StartNode);
			formula.Diameter = anEdge.DiameterInMillimeters;
			formula.Qvalue = Qvector.valueAt (anEdge);

			double velocityValue = formula.accept (formulaVisitor);

			velocityVector.atPut (anEdge, velocityValue);
		}

		#endregion
	}

}

