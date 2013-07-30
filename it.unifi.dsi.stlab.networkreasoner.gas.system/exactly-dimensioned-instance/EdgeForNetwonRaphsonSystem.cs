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
				
				var K = formula.accept(aFormulaVisitor);
				Kvector.atPut (anEdge, K);


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
			#endregion
		}

		public EdgeState SwitchState{ get; set; }

		public long Length { get; set; }

		public double DiameterInMillimeters { get; set; }

		public double RoughnessInMicron { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

		public double coVariantLittleK (
			GasFormulaVisitor aFormulaVisitor)
		{
			CovariantLittleKFormula formula = 
				new CovariantLittleKFormula ();

			formula.HeightOfStartNode = this.StartNode.Height;
			formula.HeightOfEndNode = this.EndNode.Height;

			return formula.accept (aFormulaVisitor);

		}

		public double controVariantLittleK (
			GasFormulaVisitor aFormulaVisitor)
		{
			ControVariantLittleKFormula formula = 
				new ControVariantLittleKFormula ();

			formula.HeightOfStartNode = this.StartNode.Height;
			formula.HeightOfEndNode = this.EndNode.Height;

			return formula.accept (aFormulaVisitor);


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

	}
}

