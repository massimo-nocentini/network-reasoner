using System;
using log4net;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class NewtonRaphsonSystemWithLogDecorator : NetwonRaphsonSystemInterface
	{
		ILog Log {
			get;
			set;
		}

		NetwonRaphsonSystemInterface DecoredSystemImplementation {
			get;
			set;
		}

		public NewtonRaphsonSystemWithLogDecorator (
			NetwonRaphsonSystemInterface aSystem, ILog aLog)
		{
			this.DecoredSystemImplementation = aSystem;
			this.Log = aLog;
		}

		#region NetwonRaphsonSystemInterface implementation
		public override void initializeWith (it.unifi.dsi.stlab.networkreasoner.model.gas.GasNetwork network)
		{
			this.DecoredSystemImplementation.initializeWith (network);

			this.Log.Info ("================================================================");
			this.Log.Info ("Start of a new mutation step");

			this.Log.Info ("The following nodes enumeration is used throughout the system computation:");
			foreach (var pair in this.NodesEnumeration.Value) {
				this.Log.InfoFormat ("Node: {0} -> Index: {1}", 
				                     pair.Key.Identifier, pair.Value);
			}

//			this.Log.Info ("The following edges enumeration is used throughout the system computation:");
//			foreach (var pair in this.DecoredSystemImplementation.EdgesEnumeration.Value) {
//				this.Log.InfoFormat ("(StartNode, EndNode): ({0},{1}) -> Index: {2}", 
//				                     pair.Key.StartNode.Identifier, 
//				                     pair.Key.EndNode.Identifier, 
//				                     pair.Value);
//			}
		}

		public override OneStepMutationResults repeatMutateUntil (
			System.Collections.Generic.List<UntilConditionAbstract> untilConditions)
		{
			this.Log.Info ("**********************************************************");
			this.Log.Info ("*  Start of a new computation driven by until conditions *");
			this.Log.Info ("**********************************************************");

			return this.DecoredSystemImplementation.repeatMutateUntil (untilConditions);
		}

		public override bool decideIfComputationShouldBeStopped (
			UntilConditionAbstract condition, 
			OneStepMutationResults previousOneStepMutationResults, 
			OneStepMutationResults currentOneStepMutationResults)
		{
			bool computationShouldBeStopped = this.DecoredSystemImplementation.
				decideIfComputationShouldBeStopped (condition, 
				                     previousOneStepMutationResults, 
				                     currentOneStepMutationResults);

			if (computationShouldBeStopped == true) {
				string stoppingCauseDescription = condition.stoppingCauseDescription ();
				this.Log.InfoFormat ("Computation will be stopped due to reason: {0}", 
				                    stoppingCauseDescription);
			}

			return computationShouldBeStopped;
		}

		public override OneStepMutationResults mutateWithoutIterationNumber ()
		{
			return this.DecoredSystemImplementation.mutateWithoutIterationNumber ();
		}

		public override Vector<NodeForNetwonRaphsonSystem> 
			computeUnknownVectorAtPreviousStep ()
		{
			var unknownVectorAtPreviousStep = this.DecoredSystemImplementation.
				computeUnknownVectorAtPreviousStep ();

			unknownVectorAtPreviousStep.forComputationAmong (
				this.NodesEnumeration.Value, -11010101010).stringRepresentation (
				representation => this.Log.InfoFormat (
				"Relative Unknowns at previous step: {0}", representation)
			);

			return unknownVectorAtPreviousStep;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> 
			computeFvectorAtPreviousStep ()
		{
			var FvectorAtPreviousStep = this.DecoredSystemImplementation.
				computeFvectorAtPreviousStep ();

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				FvectorAtPreviousStep, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at previous step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);

			return FvectorAtPreviousStep;
		}

		public override void fixMatricesIfSupplyGadgetsPresent (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep)
		{
			this.DecoredSystemImplementation.fixMatricesIfSupplyGadgetsPresent (
				AmatrixAtCurrentStep, JacobianMatrixAtCurrentStep);

			AmatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).stringRepresentation (
				representation => this.Log.InfoFormat ("A matrix at current step after supply node fix it:\n{0}", representation));

			JacobianMatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).stringRepresentation (
				representation => this.Log.InfoFormat ("Jacobian matrix at current step after supply node fix it:\n{0}", representation));
		}

		public override Vector<NodeForNetwonRaphsonSystem> computeCoefficientsVectorAtCurrentStep ()
		{
			var coefficientsVectorAtCurrentStep = this.DecoredSystemImplementation.
				computeCoefficientsVectorAtCurrentStep ();
			
			coefficientsVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Coefficients vector at current step: {0}", representation)
			);

			return coefficientsVectorAtCurrentStep;
		}

		public override void fixNegativeUnknowns (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep)
		{
			this.DecoredSystemImplementation.fixNegativeUnknowns (
				unknownVectorAtCurrentStep);

			unknownVectorAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, -11010101010).stringRepresentation (
				representation => this.Log.InfoFormat (
				"Adimensional unknowns vector at current step after fix negative entries: {0}", representation)
			);
		}

		public override void updatePreviousVectorsWithCurrentVectors (
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep)
		{
			this.DecoredSystemImplementation.updatePreviousVectorsWithCurrentVectors (
				unknownVectorAtCurrentStep,
				FvectorAtCurrentStep);
		}

		public override OneStepMutationResults computeOneStepMutationResult (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> QvectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> JacobianMatrixAtCurrentStep, 
			Vector<EdgeForNetwonRaphsonSystem> FvectorAtCurrentStep, 
			int? iterationNumber)
		{
			return this.DecoredSystemImplementation.computeOneStepMutationResult (
				AmatrixAtCurrentStep,
				unknownVectorAtCurrentStep,
				coefficientsVectorAtCurrentStep,
				QvectorAtCurrentStep,
				JacobianMatrixAtCurrentStep,
				FvectorAtCurrentStep,
				iterationNumber);			
		}

		public override OneStepMutationResults mutate (int? iterationNumber)
		{
			if (iterationNumber.HasValue) {
				this.Log.InfoFormat ("--------------------Iteration {0}--------------------", 
				                     iterationNumber.Value);
			}

			return this.DecoredSystemImplementation.mutate (iterationNumber);
		}

		public override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> computeAmatrix (
			Vector<EdgeForNetwonRaphsonSystem> KvectorAtCurrentStep)
		{
			var AmatrixAtCurrentStep = this.DecoredSystemImplementation.computeAmatrix (
				KvectorAtCurrentStep);

			AmatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"A matrix at current step before supply node fix it:\n{0}", representation)
			);

			return AmatrixAtCurrentStep;
		}

		public override Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> computeJacobianMatrix (
			Vector<EdgeForNetwonRaphsonSystem> kvectorAtCurrentStep)
		{
			var JacobianMatrixAtCurrentStep = this.DecoredSystemImplementation.
				computeJacobianMatrix (kvectorAtCurrentStep);

			JacobianMatrixAtCurrentStep.forComputationAmong (
				this.NodesEnumeration.Value, 
				this.NodesEnumeration.Value).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Jacobian matrix at current step before supply node fix it:\n{0}", representation)
			);

			return JacobianMatrixAtCurrentStep;

		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeKvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Fvector)
		{
			var KvectorAtCurrentStep = this.DecoredSystemImplementation.computeKvector (
				unknownVector, Fvector);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				KvectorAtCurrentStep, (edgeRepresentation, KvalueRepresentation) => 
				this.Log.InfoFormat ("K value of {0} at current step: {1}", 
			                    edgeRepresentation, KvalueRepresentation)
			)
			);

			return KvectorAtCurrentStep;
		}

		public override Vector<NodeForNetwonRaphsonSystem> computeUnknowns (
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> AmatrixAtCurrentStep, 
			Vector<NodeForNetwonRaphsonSystem> unknownVectorAtPreviousStep, 
			Vector<NodeForNetwonRaphsonSystem> coefficientsVectorAtCurrentStep, 
			Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> jacobianMatrixAtCurrentStep)
		{
			var unknownVectorAtCurrentStep = this.DecoredSystemImplementation.computeUnknowns (
				AmatrixAtCurrentStep, 
				unknownVectorAtPreviousStep, 
				coefficientsVectorAtCurrentStep, 
				jacobianMatrixAtCurrentStep);

			unknownVectorAtCurrentStep.forComputationAmong (this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Adimensional unknowns vector at current step before fix negative entries: {0}", representation)
			);

			return unknownVectorAtCurrentStep;

		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeQvector (
			Vector<NodeForNetwonRaphsonSystem> unknownVector, 
			Vector<EdgeForNetwonRaphsonSystem> Kvector)
		{
			var QvectorAtCurrentStep = this.DecoredSystemImplementation.computeQvector (unknownVector, Kvector);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				QvectorAtCurrentStep, (edgeRepresentation, QvalueRepresentation) => 
				this.Log.InfoFormat ("Q value of {0} at current step: {1}", 
			                    edgeRepresentation, QvalueRepresentation)
			)
			);

			return QvectorAtCurrentStep;
		}

		public override Vector<EdgeForNetwonRaphsonSystem> computeFvector (
			Vector<EdgeForNetwonRaphsonSystem> Fvector, 
			Vector<EdgeForNetwonRaphsonSystem> Qvector)
		{
			var FvectorAtCurrentStep = this.DecoredSystemImplementation.computeFvector (Fvector, Qvector);

			this.Edges.ForEach (anEdge => anEdge.stringRepresentationUsing (
				FvectorAtCurrentStep, (edgeRepresentation, FvalueRepresentation) => 
				this.Log.InfoFormat ("F value of {0} at current step: {1}", 
			                    edgeRepresentation, FvalueRepresentation)
			)
			);

			return FvectorAtCurrentStep;
		}

		public override Vector<NodeForNetwonRaphsonSystem> denormalizeUnknowns ()
		{
			var dimensionalUnknowns = this.DecoredSystemImplementation.denormalizeUnknowns ();

			this.UnknownVector.forComputationAmong (
				this.NodesEnumeration.Value, -11010101010).
				stringRepresentation (
					representation => this.Log.InfoFormat (
					"Relative Unknowns vector at current step: {0}", representation)
			);

			return dimensionalUnknowns;
		}

		public override it.unifi.dsi.stlab.math.algebra.Vector<EdgeForNetwonRaphsonSystem> Fvector {
			get {
				return this.DecoredSystemImplementation.Fvector;
			}
			set {
				this.DecoredSystemImplementation.Fvector = value;
			}
		}

		public override it.unifi.dsi.stlab.math.algebra.Vector<NodeForNetwonRaphsonSystem> UnknownVector {
			get {
				return this.DecoredSystemImplementation.UnknownVector;
			}
			set {
				this.DecoredSystemImplementation.UnknownVector = value;
			}
		}

		public override System.Collections.Generic.List<NodeForNetwonRaphsonSystem> Nodes {
			get {
				return this.DecoredSystemImplementation.Nodes;
			}
			set {
				this.DecoredSystemImplementation.Nodes = value;
			}
		}

		public override System.Collections.Generic.List<EdgeForNetwonRaphsonSystem> Edges {
			get {
				return this.DecoredSystemImplementation.Edges;
			}
			set {
				this.DecoredSystemImplementation.Edges = value;
			}
		}

		public override Lazy<System.Collections.Generic.Dictionary<NodeForNetwonRaphsonSystem, int>> NodesEnumeration {
			get {
				return this.DecoredSystemImplementation.NodesEnumeration;
			}
			set {
				this.DecoredSystemImplementation.NodesEnumeration = value;
			}
		}
		#endregion

	}
}

