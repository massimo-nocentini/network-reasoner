using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystem
	{
		public	interface EdgeState
		{
			void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge,
				GasFormulaVisitor aFormulaVisitor);

			void fillAmatrixUsingFor (
				Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
				Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep, 
				GasFormulaVisitor aFormulaVisitor, 
				EdgeForNetwonRaphsonSystem anEdge);

		}

		public	class EdgeStateOn:EdgeState
		{
			#region EdgeState implementation
			public  void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge,
				GasFormulaVisitor aFormulaVisitor)
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
			#endregion

			#region EdgeState implementation
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
			#endregion
		}

		public	class EdgeStateOff:EdgeState
		{
			#region EdgeState implementation
			public  void putKvalueIntoUsingFor (
				Vector<EdgeForNetwonRaphsonSystem> Kvector, 
				Vector<EdgeForNetwonRaphsonSystem> Fvector, 
				Vector<NodeForNetwonRaphsonSystem> unknownVector, 
				EdgeForNetwonRaphsonSystem anEdge,
				GasFormulaVisitor aFormulaVisitor)
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
			#endregion
		}

		public EdgeState SwitchState{ get; set; }

		public long Length { get; set; }

		public double DiameterInMillimeters { get; set; }

		public double RoughnessInMicron { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		private Double? CovariantLittleK{ get; set; }

		private Double? ControVariantLittleK{ get; set; }

		public double coVariantLittleK (
			GasFormulaVisitor aFormulaVisitor)
		{
			if (CovariantLittleK.HasValue == false) {

				CovariantLittleKFormula formula = 
					new CovariantLittleKFormula ();

				formula.HeightOfStartNode = this.StartNode.Height;
				formula.HeightOfEndNode = this.EndNode.Height;

				this.CovariantLittleK = formula.accept (aFormulaVisitor);
			}

			return this.CovariantLittleK.Value;

		}

		public double controVariantLittleK (
			GasFormulaVisitor aFormulaVisitor)
		{
			if (this.ControVariantLittleK.HasValue == false) {
				ControVariantLittleKFormula formula = 
					new ControVariantLittleKFormula ();

				formula.HeightOfStartNode = this.StartNode.Height;
				formula.HeightOfEndNode = this.EndNode.Height;

				this.ControVariantLittleK = formula.accept (aFormulaVisitor);
			}

			return this.ControVariantLittleK.Value;


		}

		public void putKvalueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			GasFormulaVisitor aFormulaVisitor)
		{
			this.SwitchState.putKvalueIntoUsingFor (
				Kvector, Fvector, unknownVector, this, aFormulaVisitor);
		}

		public void fillAmatrixUsing (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> KvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor)
		{
			this.SwitchState.fillAmatrixUsingFor (
				aMatrix, KvectorAtCurrentStep, aFormulaVisitor, this);
		}
	}
}

