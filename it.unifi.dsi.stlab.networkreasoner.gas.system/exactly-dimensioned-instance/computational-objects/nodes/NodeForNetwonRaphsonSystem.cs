using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using System.Collections.Generic;
using System.Linq;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NodeForNetwonRaphsonSystem :
	AbstractItemForNetwonRaphsonSystem, GasNodeVisitor, GasNodeGadgetVisitor
	{
		public class HeightPropertyMissingException : Exception
		{
		}

		public long Height { get; set; }

		public NodeRole Role{ get; set; }

		public AntecedentInPressureRegulation RoleInPressureRegulation{ get; set; }

		public List<EdgeForNetwonRaphsonSystem> IncomingEdges{ get; private set; }

		public List<EdgeForNetwonRaphsonSystem> OutgoingEdges{ get; private set; }

		public void initializeWith (GasNodeAbstract aNode)
		{
			this.IncomingEdges = new List<EdgeForNetwonRaphsonSystem> ();
			this.OutgoingEdges = new List<EdgeForNetwonRaphsonSystem> ();

			aNode.accept (this);

			if (this.Role == null) {
				this.Role = new NodeRolePassive ();
			}

			if (this.RoleInPressureRegulation == null) {
				this.RoleInPressureRegulation = new IsNotAntecedentInPressureRegulation ();
			}
		}

		#region GasNodeVisitor implementation

		public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
		{
			if (gasNodeTopological.Height.HasValue == false) {
				throw new HeightPropertyMissingException ();
			}
			this.Height = gasNodeTopological.Height.Value;
			this.Identifier = gasNodeTopological.Identifier;
		}

		public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
		{
			gasNodeWithGadget.Gadget.accept (this);
			gasNodeWithGadget.Equipped.accept (this);
		}

		public void forNodeAntecedentInPressureReduction (
			GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
		{
			// just for debugging we do not consider this case, since the parser
			// doesn't create any object of this type.
			throw new NotSupportedException ();

//			this.RoleInPressureRegulation = new IsAntecedentInPressureRegulation {
//				Regulator = gasNodeAntecedentInPressureRegulator.RegulatorNode
//			};
			gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
		}

		#endregion

		#region GasNodeGadgetVisitor implementation

		public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
		{
			this.Role = new NodeRoleLoader { 
				Load = aLoadGadget.Load
			};
		}

		public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
		{
			this.Role = new NodeRoleSupplier { 
				SetupPressureInMillibar = aSupplyGadget.SetupPressure
			};
		}

		#endregion

		public void putYourCoefficientInto (
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			new IfNodeIsAntecedentInPressureRegulation {
				IfItIs = data => {
					// the following computation is the opposite of the one performed
					// by FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain objects.
					var invertedAlgebraicSum = 0d;
					data.Regulator.OutgoingEdges.ForEach (edge => invertedAlgebraicSum += Qvector.valueAt (edge));
					data.Regulator.IncomingEdges.ForEach (edge => invertedAlgebraicSum -= Qvector.valueAt (edge));
					aVector.atPut (this, invertedAlgebraicSum);
				},
				Otherwise = () => this.Role.putYourCoefficientIntoFor (
					this, aVector, aFormulaVisitor, Qvector)
			}.performOn (this.RoleInPressureRegulation);

		}

		public void fixMatrixIfYouHaveSupplyGadget (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem>  aMatrix)
		{
			this.Role.fixMatrixIfYouHaveSupplyGadgetFor (this, aMatrix);
		}

		public double relativeDimensionalPressureOf (
			double absolutePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			var formula = new RelativePressureFromAdimensionalPressureFormulaForNodes ();
			formula.NodeHeight = this.Height;
			formula.AdimensionalPressure = absolutePressure;

			return formula.accept (aFormulaVisitor);
		}

		public double absoluteDimensionalPressureOf (
			double absolutePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			var formula = new AbsolutePressureFromAdimensionalPressureFormulaForNodes ();
			formula.AdimensionalPressure = absolutePressure;

			return formula.accept (aFormulaVisitor);
		}

		public GasNodeAbstract substituteNodeBecauseNegativePressureFound (
			double pressure, GasNodeAbstract originalNode)
		{
			return Role.substituteNodeBecauseNegativePressureFoundFor (
				this, pressure, originalNode);
		}

		public double fixPressureForAntecedentInReduction (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep,
			double currentValue)
		{
			double nodePressure = currentValue;

			new IfNodeIsAntecedentInPressureRegulation {
				IfItIs = data => {
					var regulatorPressure = unknownVectorAtCurrentStep.valueAt (data.Regulator);
					if (currentValue < regulatorPressure) {
						nodePressure = regulatorPressure;
					} 
				}
			}.performOn (this.RoleInPressureRegulation);
		
			return nodePressure;
		}

		public override string ToString ()
		{
			return string.Format ("[Node: id={0}]", this.Identifier);
		}
	}
}

