using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NetwonRaphsonSystem
	{
		Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		AmbientParameters AmbientParameters { get; set; }

		GasFormulaVisitor FormulaVisitor{ get; set; }

		public void useFormulaVisitor (GasFormulaVisitor aFormulaVisitor)
		{
			this.FormulaVisitor = aFormulaVisitor;
		}

		public void initializeWith (GasNetwork network)
		{
			this.AmbientParameters = network.AmbientParameters;

			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode =
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			network.doOnNodes (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aNode => {

				var newtonRaphsonNode = new NodeForNetwonRaphsonSystem ();
				newtonRaphsonNode.initializeWith (aNode);

				// un prelievo positivo implica che stiamo prelevando dalla rete
				// quindi i bilanci dei nodi di supply saranno negativi.

				newtonRaphsonNodesByOriginalNode.Add (aNode, newtonRaphsonNode);
			}
			)
			);

			List<EdgeForNetwonRaphsonSystem> collector = 
				new List<EdgeForNetwonRaphsonSystem> ();

			network.doOnEdges (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => {

				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
				aBuilder.customNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

				var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);
				collector.Add (edgeForNetwonRaphsonSystem);
			}
			)
			);

			this.Edges = collector;
		}

		public OneStepMutationResults mutate ()
		{
			var unknownVectorAtPreviousStep = UnknownVector;

			var FvectorAtPreviousStep = Fvector;

			var KvectorAtCurrentStep = computeKvector (
				unknownVectorAtPreviousStep,
				FvectorAtPreviousStep);

			var coefficientsVectorAtCurrentStep = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var AmatrixAtCurrentStep =
				computeAmatrix (KvectorAtCurrentStep);

			var JacobianMatrixAtCurrentStep =
				computeJacobianMatrix (KvectorAtCurrentStep);

			foreach (var aNode in Nodes) {

				aNode.fixMatrixIfYouHaveSupplyGadget (AmatrixAtCurrentStep);

				aNode.fixMatrixIfYouHaveSupplyGadget (JacobianMatrixAtCurrentStep);

				aNode.putYourCoefficientInto (coefficientsVectorAtCurrentStep,
				                              this.FormulaVisitor);
			}

			Vector<NodeForNetwonRaphsonSystem> matrixArightProductUnknownAtPreviousStep = 
				AmatrixAtCurrentStep.rightProduct (unknownVectorAtPreviousStep);

			Vector<NodeForNetwonRaphsonSystem> coefficientVectorForJacobianSystemFactorization = 
				matrixArightProductUnknownAtPreviousStep.minus (coefficientsVectorAtCurrentStep);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorFromJacobianSystemAtCurrentStep =
				JacobianMatrixAtCurrentStep.Solve (coefficientVectorForJacobianSystemFactorization);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				unknownVectorAtPreviousStep.minus (unknownVectorFromJacobianSystemAtCurrentStep);

			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(key, currentValue) => 
				currentValue <= 0 ? random.NextDouble () / 10 : currentValue
			);

			var QvectorAtCurrentStep = computeQvector (
				unknownVectorAtCurrentStep, 
				KvectorAtCurrentStep);

			var FvectorAtCurrentStep = computeFvector (
				FvectorAtPreviousStep, 
				QvectorAtCurrentStep);

			// here we're assuming that the initial pressure vector for unknowns 
			// is given in relative way, otherwise the following transformation
			// isn't correct because mix values with different measure unit.
			unknownVectorAtCurrentStep.updateEach (
				(aNode, absolutePressure) => 
				aNode.relativePressureOf (absolutePressure, FormulaVisitor)
			);

			this.UnknownVector = unknownVectorAtCurrentStep;
			this.Fvector = FvectorAtCurrentStep;

			var result = new OneStepMutationResults ();
			result.Amatrix = AmatrixAtCurrentStep;
			result.Unknowns = unknownVectorAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;
			result.Jacobian = JacobianMatrixAtCurrentStep;
			result.Fvector = FvectorAtCurrentStep;

			return result;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			foreach (var anEdge in this.Edges) {
			
				anEdge.fillAmatrixUsing (aMatrix, kvectorAtCurrentStep, this.FormulaVisitor);


			}

			return aMatrix;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			foreach (var anEdge in this.Edges) {
			
				var coVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.coVariantLittleK (this.FormulaVisitor);
				var controVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.controVariantLittleK (this.FormulaVisitor);

				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.StartNode, cumulate => -coVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.EndNode, cumulate => controVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.StartNode, cumulate => coVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.EndNode, cumulate => -controVariant / 2 + cumulate, 0);
			}

			return aMatrix;
		}

		Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putKvalueIntoUsing (
				Kvector, Fvector, unknownVector, this.FormulaVisitor)
			);

			return Kvector;
		}

		Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => {

				var weightedUnknownsDifference = 
					anEdge.coVariantLittleK (this.FormulaVisitor) * unknownVector.valueAt (anEdge.StartNode) -
					anEdge.controVariantLittleK (this.FormulaVisitor) * unknownVector.valueAt (anEdge.EndNode);

				Qvector.atPut (anEdge, Kvector.valueAt (anEdge) * weightedUnknownsDifference);
			}
			);

			return Qvector;
		}

		Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => {

				var numeratorForRe = 4 * Qvector.valueAt (anEdge) * 
					this.AmbientParameters.RefDensity ();
				var denominatorForRe = Math.PI * anEdge.DiameterInMillimeters * 
					AmbientParameters.ViscosityInPascalTimesSecond;
				var Re = numeratorForRe / denominatorForRe;

				var augend = anEdge.RoughnessInMicron / (anEdge.DiameterInMillimeters * 1000 * 3.71);
				var addend = 2.51 / (Re * Math.Sqrt (Fvector.valueAt (anEdge)));

				var toInvert = -2 * Math.Log10 (augend + addend);

				var Fvalue = Math.Pow (1 / toInvert, 2);

				newFvector.atPut (anEdge, Fvalue);
			}
			);

			return newFvector;
		}
	}
}

