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
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.listeners;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NetwonRaphsonSystem
	{
		public  Vector<EdgeForNetwonRaphsonSystem> Fvector{ get; set; }

		public  Vector<NodeForNetwonRaphsonSystem> UnknownVector { get; set; }

		public  List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public  List<EdgeForNetwonRaphsonSystem> Edges{ get; set; }

		public GasFormulaVisitor FormulaVisitor{ get; set; }

		public NetwonRaphsonSystemEventsListener EventsListener{ get; set; }

		public  Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration { get; set; }

		public  Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> EdgesEnumeration { get; set; }

		public Dictionary<NodeForNetwonRaphsonSystem, GasNodeAbstract> OriginalNodesByComputationNodes { get; set; }

		public Dictionary<EdgeForNetwonRaphsonSystem, GasEdgeAbstract> OriginalEdgesByComputationEdges { get; set; }

		public NetwonRaphsonSystem ()
		{
			NodesEnumeration = new Lazy<Dictionary<NodeForNetwonRaphsonSystem, int>> (
				() => this.Nodes.enumerate ());

			EdgesEnumeration = new Lazy<Dictionary<EdgeForNetwonRaphsonSystem, int>> (
				() => this.Edges.enumerate ());

			OriginalNodesByComputationNodes =
				new Dictionary<NodeForNetwonRaphsonSystem, GasNodeAbstract> ();

			OriginalEdgesByComputationEdges =
				new Dictionary<EdgeForNetwonRaphsonSystem, GasEdgeAbstract> ();

			this.EventsListener = new NetwonRaphsonSystemEventsListenerNullObject ();
		}

		#region initialization

		void initializeNodes (
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode,
			GasNetwork network)
		{
			this.UnknownVector = new Vector<NodeForNetwonRaphsonSystem> ();
			this.Nodes = new List<NodeForNetwonRaphsonSystem> ();
			
			var initialUnknownGuessVector = network.makeInitialGuessForUnknowns (new UnknownInitializationSimplyRandomized ());
			network.doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (aNode => {
				var newtonRaphsonNode = new NodeForNetwonRaphsonSystem ();
				newtonRaphsonNode.initializeWith (aNode);
				// un prelievo positivo implica che stiamo prelevando dalla rete
				// quindi i bilanci dei nodi di supply saranno negativi.
				newtonRaphsonNodesByOriginalNode.Add (aNode, newtonRaphsonNode);
				// here we get the initial unknown guess for the current node
				this.UnknownVector.atPut (newtonRaphsonNode, initialUnknownGuessVector [aNode]);
				this.OriginalNodesByComputationNodes.Add (newtonRaphsonNode, aNode);
			}
			)
			);
			this.Nodes = newtonRaphsonNodesByOriginalNode.Values.ToList ();
		}

		void initializeEdges (
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode,
			GasNetwork network)
		{
			this.Fvector = new Vector<EdgeForNetwonRaphsonSystem> ();
			var initialFvalueGuessVector = network.makeInitialGuessForFvector ();
			List<EdgeForNetwonRaphsonSystem> collector = new List<EdgeForNetwonRaphsonSystem> ();
			network.doOnEdges (new NodeHandlerWithDelegateOnRawNode<GasEdgeAbstract> (anEdge => {
				var aBuilder = new EdgeForNetwonRaphsonSystemBuilder {
					CustomNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode
				};
				var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);
				collector.Add (edgeForNetwonRaphsonSystem);
				// here we get the initial F values guess for the current node
				this.Fvector.atPut (edgeForNetwonRaphsonSystem, initialFvalueGuessVector [anEdge]);
				this.OriginalEdgesByComputationEdges.Add (edgeForNetwonRaphsonSystem, anEdge);
			}
			)
			);
			this.Edges = collector;
		}

		public void initializeWith (GasNetwork network)
		{
			// TODO: maybe this dictionary can be useful in the rest of computation?
			var newtonRaphsonNodesByOriginalNode = 
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			initializeNodes (newtonRaphsonNodesByOriginalNode, network);

			initializeEdges (newtonRaphsonNodesByOriginalNode, network);

			this.EventsListener.onInitializationCompleted (
				this.Nodes, this.Edges, this.NodesEnumeration);
		}

		#endregion

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

		public  OneStepMutationResults repeatMutateUntilRevertingDomainBack (
			List<UntilConditionAbstract> untilConditions,
			out Dictionary<GasNodeAbstract, double> unknownsByNodes, 
			out Dictionary<GasEdgeAbstract, double> QvaluesByEdges)
		{
			unknownsByNodes = new Dictionary<GasNodeAbstract, double> ();
			QvaluesByEdges = new Dictionary<GasEdgeAbstract, double> ();

			var oneStepMutationResults = this.repeatMutateUntil (untilConditions);

			var relativeUnknowns = this.makeUnknownsDimensional (oneStepMutationResults.Unknowns);
			
			foreach (var nodePair in this.OriginalNodesByComputationNodes) {
				unknownsByNodes.Add (nodePair.Value, 
				                     relativeUnknowns.valueAt (nodePair.Key));
			}

			foreach (var edgePair in this.OriginalEdgesByComputationEdges) {
				QvaluesByEdges.Add (edgePair.Value,
				                    oneStepMutationResults.Qvector.valueAt (edgePair.Key));
			}			

			return oneStepMutationResults;
		}

		public OneStepMutationResults repeatMutateUntil (
			List<UntilConditionAbstract> untilConditions)
		{
			this.EventsListener.onRepeatMutateUntilStarted ();

			OneStepMutationResults previousOneStepMutationResults = null;

			OneStepMutationResults currentOneStepMutationResults = 
			new OneStepMutationResults{
				IterationNumber = 0,
				Unknowns = this.UnknownVector
			};

			MutateComputationDriver mutateComputationDriver = 
				new MutateComputationDriverDoOneMoreMutation ();

			while (mutateComputationDriver.canDoOneMoreStep()) {			

				untilConditions.ForEach (condition => {

					if (this.decideIfComputationShouldBeStopped (condition,
					                              previousOneStepMutationResults, 
					                              currentOneStepMutationResults)) {
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

			this.EventsListener.onRepeatMutateUntilEnded (
				currentOneStepMutationResults);

			return currentOneStepMutationResults;
		}

		public bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			bool until = condition.canContinue (
				previousOneStepMutationResults, 
				currentOneStepMutationResults);

			bool computationShouldBeStopped = until == false;

			if (computationShouldBeStopped) {
				this.EventsListener.onComputationShouldBeStoppedDueTo (condition);
			}

			return computationShouldBeStopped;
		}

		public OneStepMutationResults mutateWithoutIterationNumber ()
		{
			return this.mutate (null);
		}

		public OneStepMutationResults mutate (
			Nullable<int> iterationNumber)
		{
			this.EventsListener.onMutateStepStarted (iterationNumber);

			var unknownVectorAtPreviousStep = this.computeUnknownVectorAtPreviousStep ();

			var FvectorAtPreviousStep = this.computeFvectorAtPreviousStep ();

			var KvectorAtCurrentStep = computeKvector (
				unknownVectorAtPreviousStep,
				FvectorAtPreviousStep);

			var AmatrixAtCurrentStep =
				computeAmatrix (KvectorAtCurrentStep);

			var JacobianMatrixAtCurrentStep =
				computeJacobianMatrix (KvectorAtCurrentStep);

			this.fixMatricesIfSupplyGadgetsPresent (AmatrixAtCurrentStep, JacobianMatrixAtCurrentStep);

			var coefficientsVectorAtCurrentStep = this.computeCoefficientsVectorAtCurrentStep ();

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				this.computeUnknowns (
					AmatrixAtCurrentStep, 
					unknownVectorAtPreviousStep, 
					coefficientsVectorAtCurrentStep, 
					JacobianMatrixAtCurrentStep);

			this.fixNegativeUnknowns (unknownVectorAtCurrentStep);

			var QvectorAtCurrentStep = computeQvector (
				unknownVectorAtCurrentStep, 
				KvectorAtCurrentStep);

			var FvectorAtCurrentStep = computeFvector (
				FvectorAtPreviousStep, 
				QvectorAtCurrentStep);

			updatePreviousVectorsWithCurrentVectors (
				unknownVectorAtCurrentStep, FvectorAtCurrentStep);

			var result = computeOneStepMutationResult (AmatrixAtCurrentStep, 
			                                           unknownVectorAtCurrentStep, 
			                                           coefficientsVectorAtCurrentStep, 
			                                           QvectorAtCurrentStep, 
			                                           JacobianMatrixAtCurrentStep, 
			                                           FvectorAtCurrentStep, 
			                                           iterationNumber);

			this.EventsListener.onMutateStepCompleted (result);

			return result;
		}

		#region ``compute'' methods

		public Vector<NodeForNetwonRaphsonSystem> 
			computeUnknownVectorAtPreviousStep ()
		{
			var unknownVectorAtPreviousStep = this.UnknownVector;

			this.EventsListener.onUnknownVectorAtPreviousStepComputed (
				unknownVectorAtPreviousStep);

			return unknownVectorAtPreviousStep;
		}

		public Vector<EdgeForNetwonRaphsonSystem> computeFvectorAtPreviousStep ()
		{
			var FvectorAtPreviousStep = Fvector;

			this.EventsListener.onFvectorAtPreviousStepComputed (
				FvectorAtPreviousStep);

			return FvectorAtPreviousStep;
		}

		public void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep)
		{
			this.Nodes.ForEach (aNode => {
				aNode.fixMatrixIfYouHaveSupplyGadget (AmatrixAtCurrentStep);
				aNode.fixMatrixIfYouHaveSupplyGadget (JacobianMatrixAtCurrentStep);
			}
			);

			this.EventsListener.onMatricesFixedIfSupplyGadgetsPresent (
				AmatrixAtCurrentStep, 
				JacobianMatrixAtCurrentStep);
		}

		public Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep ()
		{
			var coefficientsVectorAtCurrentStep = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			this.Nodes.ForEach (aNode => aNode.putYourCoefficientInto (
				coefficientsVectorAtCurrentStep, this.FormulaVisitor)
			);

			this.EventsListener.onCoefficientsVectorAtCurrentStepComputed (
				coefficientsVectorAtCurrentStep);

			return coefficientsVectorAtCurrentStep;
		}

		public void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			Random random = new Random ();
			unknownVectorAtCurrentStep.updateEach (
				(aNode, currentValue) => currentValue <= 0 ? 
				random.NextDouble () / 10 : currentValue
			);

			this.EventsListener.onNegativeUnknownsFixed (unknownVectorAtCurrentStep);
		}

		public void updatePreviousVectorsWithCurrentVectors (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep)
		{
			this.UnknownVector = unknownVectorAtCurrentStep;
			this.Fvector = FvectorAtCurrentStep;
		}

		public OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int? iterationNumber)
		{
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

		public  Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeAmatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.fillAmatrixUsing (
				Amatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

			this.EventsListener.onAmatrixComputed (Amatrix);

			return Amatrix;
		}

		public Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> 
			computeJacobianMatrix (Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrix =
				new Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.fillJacobianMatrixUsing (
				JacobianMatrix, kvectorAtCurrentStep, this.FormulaVisitor)
			);

			this.EventsListener.onJacobianMatrixComputed (JacobianMatrix);

			return JacobianMatrix;
		}

		public Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector,
			Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			var Kvector = new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putKvalueIntoUsing (
				Kvector, Fvector, unknownVector, this.FormulaVisitor)
			);

			this.EventsListener.onKvectorComputed (Kvector);

			return Kvector;
		}

		public Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
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

			unknownVectorFromJacobianSystemAtCurrentStep.updateEach (
				(node, currentValue) => currentValue * .75);

			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep = 
				unknownVectorAtPreviousStep.minus (unknownVectorFromJacobianSystemAtCurrentStep);

			this.EventsListener.onUnknownVectorAtCurrentStepComputed (unknownVectorAtCurrentStep);

			return unknownVectorAtCurrentStep;
		}

		public Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> Qvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putQvalueIntoUsing (
				Qvector, Kvector, unknownVector, this.FormulaVisitor)
			);

			this.EventsListener.onQvectorComputed (Qvector);

			return Qvector;
		}

		public Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			Vector<EdgeForNetwonRaphsonSystem> newFvector = 
				new Vector<EdgeForNetwonRaphsonSystem> ();

			this.Edges.ForEach (anEdge => anEdge.putNewFvalueIntoUsing (
				newFvector, Qvector, Fvector, this.FormulaVisitor)
			);

			this.EventsListener.onFvectorAtCurrentStepComputed (newFvector);

			return newFvector;
		}

		#endregion

		public Vector<NodeForNetwonRaphsonSystem> makeUnknownsDimensional (
			Vector<NodeForNetwonRaphsonSystem> adimensionalUnknowns)
		{
			Vector<NodeForNetwonRaphsonSystem> dimensionalUnknowns = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			this.Nodes.ForEach (aNode => {

				double adimensionalPressure = adimensionalUnknowns.valueAt (aNode);

				double dimensionalPressure = aNode.dimensionalPressureOf (
					adimensionalPressure, this.FormulaVisitor);

				dimensionalUnknowns.atPut (aNode, dimensionalPressure);

			}
			);

			this.EventsListener.onUnknownWithDimensionReverted (dimensionalUnknowns);

			return this.UnknownVector;
		}

		public OneStepMutationResults fixNodesWithLoadGadgetNegativePressure (
			OneStepMutationResults previousMutationResults, 
			List<UntilConditionAbstract> untilConditions,
			Dictionary<GasNodeAbstract, GasNodeAbstract> fixedNodesWithLoadGadgetByOriginalNodes)
		{
			var relativeUnknowns = this.makeUnknownsDimensional (previousMutationResults.Unknowns);

			var nodeWithMinValue = relativeUnknowns.findKeyWithMinValue ();

			// here we don't know if the pressure is negative or not.
			var originalNode = OriginalNodesByComputationNodes [nodeWithMinValue];

			NodeForNetwonRaphsonSystem.NodeSubstitutionAbstract substitutionDriver = 
					nodeWithMinValue.substituteNodeIfHasNegativePressure (
						relativeUnknowns.valueAt (nodeWithMinValue),
						originalNode);

			OneStepMutationResults resultAfterFixingOneNodeWithLoadGadget =
				substitutionDriver.doSubstitution (
					previousMutationResults,
					originalNode,
					fixedNodesWithLoadGadgetByOriginalNodes,
					this.OriginalEdgesByComputationEdges,
					untilConditions);

			return substitutionDriver.continueComputationFor (
				this, 
				resultAfterFixingOneNodeWithLoadGadget, 
				untilConditions, 
				fixedNodesWithLoadGadgetByOriginalNodes);

		}

	}
}

