using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using log4net;
using log4net.Config;
using System.IO;
using System.Linq;
using it.unifi.dsi.stlab.extensionmethods;
using System.Globalization;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NetwonRaphsonSystem
	{
		Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		public GasFormulaVisitor FormulaVisitor{ get; set; }

		public ILog Log{ get; set; }
		
		Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration { get; set; }

		Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> EdgesEnumeration { get; set; }

		public NetwonRaphsonSystem ()
		{
			NodesEnumeration = new Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> (
				() => this.Nodes.enumerate ());

			EdgesEnumeration = new Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> (
				() => this.Edges.enumerate ());

		}

		public void writeSomeLog (String infoMessage)
		{
			this.Log.Info (infoMessage);
		}

		public void initializeWith (GasNetwork network)
		{

			this.UnknownVector = new Vector<NodeForNetwonRaphsonSystem> ();
			this.Fvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode =
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			var initialUnknownGuessVector = network.makeInitialGuessForUnknowns ();
			var initialFvalueGuessVector = network.makeInitialGuessForFvector ();

			network.doOnNodes (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aNode => {

				var newtonRaphsonNode = new NodeForNetwonRaphsonSystem ();
				newtonRaphsonNode.initializeWith (aNode);

				// un prelievo positivo implica che stiamo prelevando dalla rete
				// quindi i bilanci dei nodi di supply saranno negativi.

				newtonRaphsonNodesByOriginalNode.Add (aNode, newtonRaphsonNode);

				// here we get the initial unknown guess for the current node
				this.UnknownVector.atPut (newtonRaphsonNode,
				                         initialUnknownGuessVector [aNode]);
			}
			)
			);

			this.Nodes = newtonRaphsonNodesByOriginalNode.Values.ToList ();

			List<EdgeForNetwonRaphsonSystem> collector = 
				new List<EdgeForNetwonRaphsonSystem> ();

			network.doOnEdges (new GasNetwork.NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (
				anEdge => {

				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
				aBuilder.customNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

				var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);
				collector.Add (edgeForNetwonRaphsonSystem);

				// here we get the initial F values guess for the current node
				this.Fvector.atPut (edgeForNetwonRaphsonSystem,
				                    initialFvalueGuessVector [anEdge]);
			}
			)
			);

			this.Edges = collector;
		}

		abstract class MutateComputationDriver
		{
			public abstract bool canDoOneMoreStep ();

			public abstract void doOneMoreStep (Action step);
		}

		class MutateComputationDriverDoOneMoreMutation : MutateComputationDriver
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver
			public override bool canDoOneMoreStep ()
			{
				return true;
			}

			public override void doOneMoreStep (Action step)
			{
				step.Invoke ();
			}
			#endregion
		}

		class MutateComputationDriverStopMutate : MutateComputationDriver
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.NetwonRaphsonSystem.MutateComputationDriver
			public override bool canDoOneMoreStep ()
			{
				return false;
			}

			public override void doOneMoreStep (Action step)
			{
				// nothing to do with step since we've to interrupt the computation
			}
			#endregion
		}

		public OneStepMutationResults repeatMutateUntil (
			List<UntilConditionAbstract> untilConditions)
		{
			this.Log.Info ("================================================================");
			this.Log.Info ("Start of a new mutation step");

			this.Log.Info ("The following nodes enumeration is used throughout the system computation:");
			foreach (var pair in this.NodesEnumeration.Value) {
				this.Log.InfoFormat ("Node: {0} -> Index: {1}", 
				                     pair.Key.Identifier, pair.Value);
			}

			this.Log.Info ("The following edges enumeration is used throughout the system computation:");
			foreach (var pair in this.EdgesEnumeration.Value) {
				this.Log.InfoFormat ("(StartNode, EndNode): ({0},{1}) -> Index: {2}", 
				                     pair.Key.StartNode.Identifier, 
				                     pair.Key.EndNode.Identifier, 
				                     pair.Value);
			}

			OneStepMutationResults previousOneStepMutationResults = null;

			OneStepMutationResults currentOneStepMutationResults = 
			new OneStepMutationResults{
				IterationNumber = 0
			};

			MutateComputationDriver mutateComputationDriver = 
				new MutateComputationDriverDoOneMoreMutation ();

			while (mutateComputationDriver.canDoOneMoreStep()) {			

				untilConditions.ForEach (condition => {

					if (this.CheckUntilCondition (() => condition.canContinue (
							previousOneStepMutationResults, currentOneStepMutationResults)
					)) {
						mutateComputationDriver = new MutateComputationDriverStopMutate ();
					}
				}
				);

				mutateComputationDriver.doOneMoreStep (
					step: () => {

					previousOneStepMutationResults = currentOneStepMutationResults;

					currentOneStepMutationResults = this.mutate (
						previousOneStepMutationResults.IterationNumber + 1);
				}
				);
			}

			return currentOneStepMutationResults;
		}

		public bool CheckUntilCondition (Func<bool> condition)
		{
			return condition.Invoke ();
		}

		public OneStepMutationResults mutateWithoutIterationNumber ()
		{
			return this.mutate (null);
		}

		public OneStepMutationResults mutate (
			Nullable<int> iterationNumber)
		{
			if (iterationNumber.HasValue) {
				this.Log.InfoFormat ("--------------------Iteration {0}--------------------", 
				                     iterationNumber.Value);
			}

			var unknownVectorAtPreviousStep = UnknownVector;

			unknownVectorAtPreviousStep.forComputationAmong (
				this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Relative Unknowns at previous step: {0}", representation)
			);

			var FvectorAtPreviousStep = Fvector;

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				FvectorAtPreviousStep, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at previous step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);

			var KvectorAtCurrentStep = computeKvector (
				unknownVectorAtPreviousStep,
				FvectorAtPreviousStep);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				KvectorAtCurrentStep, (edgeRepresentation, KvalueRepresentation) => 
				this.Log.InfoFormat ("K value of {0} at current step: {1}", 
			                    edgeRepresentation, KvalueRepresentation)
			)
			);

			var coefficientsVectorAtCurrentStep = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var AmatrixAtCurrentStep =
				computeAmatrix (KvectorAtCurrentStep);

			AmatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"A matrix at current step before supply node fix it:\n{0}", representation)
			);

			var JacobianMatrixAtCurrentStep =
				computeJacobianMatrix (KvectorAtCurrentStep);

			JacobianMatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Jacobian matrix at current step before supply node fix it:\n{0}", representation)
			);

			foreach (var aNode in Nodes) {

				aNode.fixMatrixIfYouHaveSupplyGadget (AmatrixAtCurrentStep);

				aNode.fixMatrixIfYouHaveSupplyGadget (JacobianMatrixAtCurrentStep);

				aNode.putYourCoefficientInto (coefficientsVectorAtCurrentStep,
				                              this.FormulaVisitor);
			}

			AmatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"A matrix at current step after supply node fix it:\n{0}", representation)
			);

			JacobianMatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Jacobian matrix at current step after supply node fix it:\n{0}", representation)
			);

			coefficientsVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Coefficients vector at current step: {0}", representation)
			);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				this.computeUnknowns (
					AmatrixAtCurrentStep, 
					unknownVectorAtPreviousStep, 
					coefficientsVectorAtCurrentStep, 
					JacobianMatrixAtCurrentStep);

			unknownVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Absolute Unknowns vector at current step before fix negative entries: {0}", representation)
			);

			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => 
				currentValue <= 0 ? random.NextDouble () / 10 : currentValue
			);

			unknownVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Absolute Unknowns vector at current step after fix negative entries: {0}", representation)
			);

			var QvectorAtCurrentStep = computeQvector (
				unknownVectorAtCurrentStep, 
				KvectorAtCurrentStep);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				QvectorAtCurrentStep, (edgeRepresentation, QvalueRepresentation) => 
				this.Log.InfoFormat ("Q value of {0} at current step: {1}", 
			                    edgeRepresentation, QvalueRepresentation)
			)
			);

			var FvectorAtCurrentStep = computeFvector (
				FvectorAtPreviousStep, 
				QvectorAtCurrentStep);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				FvectorAtCurrentStep, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at current step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);

//			// here we're assuming that the initial pressure vector for unknowns 
//			// is given in relative way, otherwise the following transformation
//			// isn't correct because mix values with different measure unit.
//			unknownVectorAtCurrentStep.updateEach (
//				(aNode, absolutePressure) => 
//				aNode.relativePressureOf (absolutePressure, this.FormulaVisitor)
//			);
//
//			unknownVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
//				stringRepresentation (
//					representation => this.Log.InfoFormat (
//					"Relative Unknowns vector at current step: {0}", representation)
//			);

			this.UnknownVector = unknownVectorAtCurrentStep;
			this.Fvector = FvectorAtCurrentStep;

			var result = new OneStepMutationResults ();
			result.Amatrix = AmatrixAtCurrentStep;
			result.Unknowns = unknownVectorAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;
			result.Jacobian = JacobianMatrixAtCurrentStep;
			result.Fvector = FvectorAtCurrentStep;

			if (iterationNumber.HasValue) {
				result.IterationNumber = iterationNumber.Value;
			}

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
				jacobianMatrixAtCurrentStep.SolveWithGivenEnumerations (
					this.NodesEnumeration.Value,
					this.NodesEnumeration.Value,
					coefficientVectorForJacobianSystemFactorization);

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

		public Vector<NodeForNetwonRaphsonSystem> denormalizeUnknowns ()
		{
			
			// here we're assuming that the initial pressure vector for unknowns 
			// is given in relative way, otherwise the following transformation
			// isn't correct because mix values with different measure unit.
			this.UnknownVector.updateEach (
				(aNode, absolutePressure) => 
				aNode.relativePressureOf (absolutePressure, this.FormulaVisitor)
			);

			this.UnknownVector.forComputationAmong (
				this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Relative Unknowns vector at current step: {0}", representation)
			);

			return this.UnknownVector;
		}

	}
}

