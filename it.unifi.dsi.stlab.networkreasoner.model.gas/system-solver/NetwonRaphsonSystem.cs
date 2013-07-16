using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetwonRaphsonSystem
	{
		private Dictionary<GasEdge, Double> Qvector{ get; set; }

		private Dictionary<NodeMatrixConstruction, double> Unknowns { get; set; }

		private List<NodeMatrixConstruction> Nodes{ get; set; }

		private List<GasEdge> Edges{ get; set; }

		private MatrixComputationDataProvider ComputationDataProvider { get; set; }

		private Dictionary<KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double>
			LittleK { get; set; }

		public NetwonRaphsonSystem ()
		{
			// TODO: set Nodes and Edges and initialize the computationDataProvider
		}

		public void InitialGuessForQvector (
			Dictionary<GasEdge, Double> Qvector)
		{
			this.Qvector = Qvector;
		}

		public void InitialGuessForUnknowns (
			Dictionary<NodeMatrixConstruction, double> unknowns)
		{
			this.Unknowns = unknowns;
		}

		public OneStepMutationResults mutate ()
		{
			var unknownsAtPreviousStep = Unknowns;
			var QvectorAtPreviousStep = Qvector;
			Dictionary<GasEdge, Double> VvectorAtPreviousStep = null;





			var KvectorAtCurrentStep = computeKvector (
				unknownsAtPreviousStep,
				QvectorAtPreviousStep,
				null,
				null);

			var coefficientsVectorAtCurrentStep = new Dictionary<NodeMatrixConstruction, double> ();

			var matrixAtCurrentStep = new Dictionary<
				KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double> ();

			var dataProviderAtCurrentStep = new MatrixComputationDataProviderDictionaryImplementation (
				LittleK, KvectorAtCurrentStep);

			foreach (var nodeInRow in Nodes) {
				foreach (var nodeInColumn in Nodes) {

					var nodePair = new KeyValuePair<
						NodeMatrixConstruction,NodeMatrixConstruction> (
							nodeInRow, nodeInColumn);

					double value = nodeInRow.matrixValueRespect (
						nodeInColumn, dataProviderAtCurrentStep);

					matrixAtCurrentStep.Add (nodePair, value);

				}

				coefficientsVectorAtCurrentStep [nodeInRow] = nodeInRow.coefficient ();
			}

			var unknownsAtCurrentStep = Solve (
				matrixAtCurrentStep, coefficientsVectorAtCurrentStep);

			var QvectorAtCurrentStep = computeQvector (
				unknownsAtCurrentStep, dataProviderAtCurrentStep);

			Unknowns = unknownsAtCurrentStep;
			Qvector = QvectorAtCurrentStep;

			var result = new OneStepMutationResults ();

			result.Matrix = matrixAtCurrentStep;
			result.Unknowns = unknownsAtCurrentStep;
			result.Coefficients = coefficientsVectorAtCurrentStep;
			result.Qvector = QvectorAtCurrentStep;

			return result;
		}

		protected Dictionary<KeyValuePair<NodeMatrixConstruction,NodeMatrixConstruction>, Double> 
			computeKvector (
			Dictionary<NodeMatrixConstruction, double> unknowns,
			Dictionary<GasEdge, Double> Qvector, // questo possiamo non passarlo
			Dictionary<GasEdge, Double> Vvector, // questo possiamo non passarlo
			Dictionary<GasEdge, Double> Fvector) // di default 0.015, per ogni arco
		{
			foreach (var edge in this.Edges) {
				var f = Fvector[edge];
				var A = 4837.00;
				var l = edge.Length;
				//unknowns[edge.StartNode.adapterForMatrixConstruction()] - unknowns[edge.EndNode.adapterForMatrixConstruction()]
			}

			return null;
		}

		protected Dictionary<NodeMatrixConstruction, double> Solve (
			Dictionary<KeyValuePair<NodeMatrixConstruction, NodeMatrixConstruction>, double> matrixAtCurrentStep, 
			Dictionary<NodeMatrixConstruction, double> coefficientsVector)
		{
			throw new NotImplementedException ();
		}

		protected Dictionary<GasEdge, Double> computeQvector (
			Dictionary<NodeMatrixConstruction, double> unknownsAtCurrentStep, 
			MatrixComputationDataProviderDictionaryImplementation dataProviderAtCurrentStep)
		{
			throw new NotImplementedException ();
		}



	}
}

