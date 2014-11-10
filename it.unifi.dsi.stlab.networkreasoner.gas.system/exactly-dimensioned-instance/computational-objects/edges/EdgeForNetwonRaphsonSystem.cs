using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystem : AbstractItemForNetwonRaphsonSystem
	{
		public EdgeState SwitchState{ get; set; }

		public EdgeRegulator RegulatorState { get; set; }

		public double Length { get; set; }

		public double DiameterInMillimeters { get; set; }

		public double RoughnessInMicron { get; set; }

		public NodeForNetwonRaphsonSystem StartNode{ get; set; }

		public NodeForNetwonRaphsonSystem EndNode{ get; set; }

		private Double? CovariantLittleK{ get; set; }

		private Double? ControVariantLittleK{ get; set; }

		public string identifierUsingLinkNotation ()
		{
			return StartNode.Identifier + " -> " + EndNode.Identifier;
		}

		public override string ToString ()
		{
			return string.Format ("[EdgeForNetwonRaphsonSystem: Link={0}]", 
				identifierUsingLinkNotation ());
		}

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
			new IfEdgeHasntRegulatorGadget {
				Do = () => {
					this.SwitchState.putKvalueIntoUsingFor (
						Kvector, Fvector, unknownVector, aFormulaVisitor, this);
				}
			}.performOn (this.RegulatorState);

		}

		public void fillAmatrixUsing (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> KvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor)
		{
			new IfEdgeHasntRegulatorGadget {
				Do = () => {
					this.SwitchState.fillAmatrixUsingFor (
						aMatrix, KvectorAtCurrentStep, aFormulaVisitor, this);
				}
			}.performOn (this.RegulatorState);
		}

		public void fillJacobianMatrixUsing (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aJacobianMatrix, 
			Vector<EdgeForNetwonRaphsonSystem> KvectorAtCurrentStep, 
			GasFormulaVisitor aFormulaVisitor)
		{
			new IfEdgeHasntRegulatorGadget {
				Do = () => {
					this.SwitchState.fillJacobianMatrixFor (
						aJacobianMatrix, KvectorAtCurrentStep, aFormulaVisitor, this);
				}
			}.performOn (this.RegulatorState);
		}

		public void putQvalueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector, 
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			GasFormulaVisitor aFormulaVisitor)
		{
			new IfEdgeHasntRegulatorGadget {
				Do = () => {
					this.SwitchState.putQvalueIntoUsingFor (
						Qvector, Kvector, unknownVector, aFormulaVisitor, this);
				}
			}.performOn (this.RegulatorState);
		}

		public void putNewFvalueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem> newFvector,
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			Vector<EdgeForNetwonRaphsonSystem> previousFvector, 
			GasFormulaVisitor formulaVisitor)
		{
			new IfTrueIfFalseEdgeHasRegulatorGadget {

				IfTrue = () => {
					newFvector.atPut (this, 0d);					
				},

				IfFalse = () => {
					this.SwitchState.putNewFvalueIntoUsingFor (
						newFvector, Qvector, previousFvector, formulaVisitor, this);
				}

			}.performOn (this.RegulatorState);

		}

		public void stringRepresentationUsing (
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtPreviousStep, 
			Action<String, String> continuation)
		{
			this.SwitchState.stringRepresentationUsingFor (
				FvectorAtPreviousStep, continuation, this);
		}

		public String topologicalStringRepresentation ()
		{
			return string.Format ("({0} -> {1})", 
				this.StartNode.Identifier, 
				this.EndNode.Identifier);
		}

		public void putVelocityValueIntoUsing (
			Vector<EdgeForNetwonRaphsonSystem> velocityVector, 
			Vector<NodeForNetwonRaphsonSystem> pressures, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector, 
			GasFormulaVisitor formulaVisitor)
		{

			new IfTrueIfFalseEdgeHasRegulatorGadget {

				IfTrue = () => velocityVector.atPut (this, 0d),

				IfFalse = () => {
					this.SwitchState.putVelocityValueIntoUsingFor (
						velocityVector, pressures, Qvector, formulaVisitor, this);
				}
			}.performOn (this.RegulatorState);

		}


		public double fetchFlowFromQvector (Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			double? flow = null;

			new IfTrueIfFalseEdgeHasRegulatorGadget {

				IfTrue = () => flow = 0d,

				IfFalse = () => flow = Qvector.valueAt (this)

			}.performOn (this.RegulatorState);

			// here we don't check if `flow' has a value since this style
			// is an instance of self checking code, ie it is mandatory
			// that `flow' would be set inside the IfTrueIfFalseEdgeHasRegulatorGadget
			// dispatched predicate.
			return flow.Value;
		}
	}
}

