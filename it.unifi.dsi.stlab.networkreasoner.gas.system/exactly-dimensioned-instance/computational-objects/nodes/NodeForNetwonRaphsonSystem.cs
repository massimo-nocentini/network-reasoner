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

		public virtual double invertedAlgebraicFlowSum (
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			// the following computation is the opposite of the one performed
			// by FluidDynamicSystemStateVisitorRevertComputationResultsOnOriginalDomain objects.
			var invertedAlgebraicSum = 0d;

			this.OutgoingEdges.ForEach (
				edge => invertedAlgebraicSum += edge.fetchFlowFromQvector (Qvector));

			this.IncomingEdges.ForEach (
				edge => invertedAlgebraicSum -= edge.fetchFlowFromQvector (Qvector));

			return invertedAlgebraicSum;
		}

		public void putYourCoefficientInto (
			Vector<NodeForNetwonRaphsonSystem> aVector,
			GasFormulaVisitor aFormulaVisitor,
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			new IfNodeIsAntecedentInPressureRegulation {

				IfItIs = data => aVector.atPut (
					this, data.Regulator.invertedAlgebraicFlowSum (Qvector)),

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

		public double fixPressureIfAntecedentInPressureReductionRelation (
			double theCurrentNodePressure,
			GasFormulaVisitor aFormulaVisitor)
		{
			double antecedentNodePressure = theCurrentNodePressure;

			new IfNodeIsAntecedentInPressureRegulation {
				IfItIs = data => {
				
					if (antecedentNodePressure < 1d) {

						// restore atmospheric pressure
						antecedentNodePressure = 1d;

						// I don't understand if the following is the correct one
//						antecedentNodePressure =	this.relativeDimensionalPressureOf (1d, aFormulaVisitor);
					}
				}
			}.performOn (this.RoleInPressureRegulation);
		
			return antecedentNodePressure;
		}

		public double fixPressureIfInPressureReductionRelation (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep,
			double theCurrentNodePressure,
			Func<NodeForNetwonRaphsonSystem, GasNodeAbstract> aNodeMapper,
			GasFormulaVisitor aFormulaVisitor)
		{
			double nodePressure = theCurrentNodePressure;

			new IfNodeIsConsequentInPressureRegulation {
				IfItIs = data => {

					var antecedentNodePressure = unknownVectorAtCurrentStep.valueAt (data.Antecedent);

					// the following assertion is assured by a precedent method 
					// invocation that fix exactly this condition. We move that logic
					// in a dedicated chunck of code since here we cannot update the value
					// for the antecedent node, because this method should be 
					// invoked inside a foreach loop.
					if (antecedentNodePressure < 1d) {
						throw new NotSupportedException ("Antecedent pressure cannot be less than 1d.");
					}

					//		if (antecedentNodePressure < currentNodePressure) {

					var originalSetupPressure = new SetupPressureFinder {
						FormulaVisitor = aFormulaVisitor
					}.of (aNodeMapper.Invoke (this));

					nodePressure = Math.Min (antecedentNodePressure, originalSetupPressure);

					var newRelativePressure = this.relativeDimensionalPressureOf (
						                          nodePressure, aFormulaVisitor);

					if (newRelativePressure < -1000) {
						Console.WriteLine ("Relative pressure under -1000");
					}

					this.substituteSupplyGadgetBecauseAntecedentInPressureRegulationHasLowerPressure (
						newRelativePressure,
						aNodeMapper);
					//} 
				}
			}.performOn (this.RoleInPressureRegulation);
		
			return nodePressure;
		}

		class SetupPressureFinder : GasNodeVisitor, GasNodeGadgetVisitor
		{
			public GasFormulaVisitor FormulaVisitor{ get; set; }

			public double? OriginalSetupPressure { get; set; }

			public long? Height{ get; set; }

			public Double of (GasNodeAbstract gasNodeAbstract)
			{
				gasNodeAbstract.accept (this);

				var coefficientFormula = new CoefficientFormulaForNodeWithSupplyGadget {
					GadgetSetupPressureInMillibar = this.OriginalSetupPressure.Value,
					NodeHeight = this.Height.Value				 
				};

				return coefficientFormula.accept (this.FormulaVisitor);

			}

			#region GasNodeGadgetVisitor implementation

			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				throw new NotSupportedException ();
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				this.OriginalSetupPressure = aSupplyGadget.SetupPressure;
			}

			#endregion

			#region GasNodeVisitor implementation

			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				this.Height = gasNodeTopological.Height;
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Equipped.accept (this);
				gasNodeWithGadget.Gadget.accept (this);
			}

			public void forNodeAntecedentInPressureReduction (
				GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
			{
				gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
			}

			#endregion


		}

		void substituteSupplyGadgetBecauseAntecedentInPressureRegulationHasLowerPressure (
			double newSetupPressure, 
			Func<NodeForNetwonRaphsonSystem, GasNodeAbstract> originalNodeMapper)
		{
		
			this.Role = new NodeRoleSupplier { 
				SetupPressureInMillibar = newSetupPressure 
			};

			// maybe this update on the original node can 
			// be skipped since the user provide the
			// value. In subsequent computation, for instance
			// in negative pressure on load nodes, do we have
			// to use this fixed value or not?
//			GasNodeAbstract correspondingOriginalNode = 
//				originalNodeMapper.Invoke (this);
//
//			correspondingOriginalNode.accept (
//				new SubstituteSupplyGadget{ SetupPressure = newSetupPressure });
		}

		public override string ToString ()
		{
			return string.Format ("[Node: id={0}]", this.Identifier);
		}

		class SubstituteSupplyGadget : GasNodeVisitor, GasNodeGadgetVisitor
		{
			public Double SetupPressure{ get; set; }

			#region GasNodeGadgetVisitor implementation

			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				throw new NotSupportedException ("This method shouldn't be called since we're " +
				"substituting a node with a supply gadget.");
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				aSupplyGadget.SetupPressure = SetupPressure;
			}

			#endregion

			#region GasNodeVisitor implementation

			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				// nothing to do
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}

			public void forNodeAntecedentInPressureReduction (GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
			{
				gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
			}

			#endregion


		}
	}
}

