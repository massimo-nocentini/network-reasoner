using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NetwonRaphsonSystem
	{
		Vector<EdgeForNetwonRaphsonSystem, Double> Fvector{ get; set; }

		Vector<NodeForNetwonRaphsonSystem, Double> UnknownVector { get; set; }

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
				aBuilder.AmbientParameters = network.AmbientParameters;
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
				new Vector<NodeForNetwonRaphsonSystem, Double> ();

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

			Vector<NodeForNetwonRaphsonSystem, Double> matrixArightProductUnknownAtPreviousStep = 
				AmatrixAtCurrentStep.rightProduct (unknownVectorAtPreviousStep);

			Vector<NodeForNetwonRaphsonSystem, Double> coefficientVectorForJacobianSystemFactorization = 
				matrixArightProductUnknownAtPreviousStep.minus (coefficientsVectorAtCurrentStep);

			Vector<NodeForNetwonRaphsonSystem, Double> unknownVectorFromJacobianSystemAtCurrentStep =
				JacobianMatrixAtCurrentStep.Solve (coefficientVectorForJacobianSystemFactorization);

			Vector<NodeForNetwonRaphsonSystem, Double> unknownVectorAtCurrentStep = 
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

			this.UnknownVector = unknownVectorAtCurrentStep;
			this.Fvector = FvectorAtCurrentStep;

			// alla fine di ogni passo di mutate su tutto il vettore
			// delle unknown (quindi sia per nodi di supply che di load)
			// dobbiamo fare il procedimento inverso per restituire le pressioni relative.

			var result = new OneStepMutationResults ();
			result.Amatrix = AmatrixAtCurrentStep;
			result.Unknowns = unknownVectorAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;
			result.Jacobian = JacobianMatrixAtCurrentStep;
			result.Fvector = FvectorAtCurrentStep;

			return result;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem, double> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> ();

			foreach (var anEdge in this.Edges) {
			
				var coVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.coVariantLittleK ();
				var controVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.controVariantLittleK ();

				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.StartNode, cumulate => -coVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.EndNode, cumulate => controVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.StartNode, cumulate => coVariant + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.EndNode, cumulate => -controVariant + cumulate, 0);
			}

			return aMatrix;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem, double> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> ();

			foreach (var anEdge in this.Edges) {
			
				var coVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.coVariantLittleK ();
				var controVariant = kvectorAtCurrentStep.valueAt (anEdge) * anEdge.controVariantLittleK ();

				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.StartNode, cumulate => -coVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.StartNode, anEdge.EndNode, cumulate => controVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.StartNode, cumulate => coVariant / 2 + cumulate, 0);
				aMatrix.atRowAtColumnPut (anEdge.EndNode, anEdge.EndNode, cumulate => -controVariant / 2 + cumulate, 0);
			}

			return aMatrix;
		}

		Vector<EdgeForNetwonRaphsonSystem, Double> computeKvector (
			Vector<NodeForNetwonRaphsonSystem, Double> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem, Double> Fvector)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem, double> ();

			this.Edges.ForEach (anEdge => anEdge.putKvalueIntoUsing (
				Kvector, Fvector, unknownVector)
			);

			return Kvector;
		}

		Vector<EdgeForNetwonRaphsonSystem, double> computeQvector (
			Vector<NodeForNetwonRaphsonSystem, double> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem, double> Kvector)
		{
			Vector<EdgeForNetwonRaphsonSystem, double> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem, double> ();

			this.Edges.ForEach (anEdge => {

				var weightedUnknownsDifference = 
					anEdge.coVariantLittleK () * unknownVector.valueAt (anEdge.StartNode) -
					anEdge.controVariantLittleK () * unknownVector.valueAt (anEdge.EndNode);

				Qvector.atPut (anEdge, Kvector.valueAt (anEdge) * weightedUnknownsDifference);
			}
			);

			return Qvector;
		}

		Vector<EdgeForNetwonRaphsonSystem, double> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem, double> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem, double> Qvector)
		{
			Vector<EdgeForNetwonRaphsonSystem, double> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem, double> ();

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

