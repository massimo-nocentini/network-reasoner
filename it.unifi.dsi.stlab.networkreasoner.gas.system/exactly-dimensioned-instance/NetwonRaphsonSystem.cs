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

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				this.computeUnknowns (
					AmatrixAtCurrentStep, 
					unknownVectorAtPreviousStep, 
					coefficientsVectorAtCurrentStep, 
					JacobianMatrixAtCurrentStep);

			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => 
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
				aNode.relativePressureOf (absolutePressure, this.FormulaVisitor)
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

			this.Edges.ForEach (anEdge => anEdge.fillAmatrixUsing (
				aMatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

			return aMatrix;
		}

		Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> aMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.fillJacobianMatrixUsing (
				aMatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

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

		Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep)
		{
			Vector<NodeForNetwonRaphsonSystem> matrixArightProductUnknownAtPreviousStep = 
				AmatrixAtCurrentStep.rightProduct (unknownVectorAtPreviousStep);

			Vector<NodeForNetwonRaphsonSystem> coefficientVectorForJacobianSystemFactorization = 
				matrixArightProductUnknownAtPreviousStep.minus (coefficientsVectorAtCurrentStep);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorFromJacobianSystemAtCurrentStep =
				jacobianMatrixAtCurrentStep.Solve (coefficientVectorForJacobianSystemFactorization);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				unknownVectorAtPreviousStep.minus (unknownVectorFromJacobianSystemAtCurrentStep);

			return unknownVectorAtCurrentStep;
		}

		Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putQvalueIntoUsing (
				Qvector, Kvector, unknownVector, this.FormulaVisitor)
			);

			return Qvector;
		}

		Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putNewFvalueIntoUsing (
				newFvector, Qvector, Fvector, this.FormulaVisitor)
			);

			return newFvector;
		}
	}
}

