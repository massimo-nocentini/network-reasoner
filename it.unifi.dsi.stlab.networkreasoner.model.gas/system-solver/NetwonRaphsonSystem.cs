using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class NetwonRaphsonSystem
	{
		private Dictionary<GasEdge, Double> Qvector{ get; set; }

		private Dictionary<GasNodeAbstract, double> Unknowns { get; set; }

		private List<GasNodeAbstract> Nodes{ get; set; }

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
			Dictionary<GasNodeAbstract, double> unknowns)
		{
			this.Unknowns = unknowns;
		}

		public OneStepMutationResults mutate ()
		{
			var unknownsAtPreviousStep = Unknowns;
			var QvectorAtPreviousStep = Qvector;

			var KvectorAtCurrentStep = computeKvector (
				unknownsAtPreviousStep,
				QvectorAtPreviousStep);

			var matrixAtCurrentStep = new Dictionary<
				KeyValuePair<GasNodeAbstract,GasNodeAbstract>, Double> ();

			foreach (var nodeInRow in Nodes) {
				foreach (var nodeInColumn in Nodes) {

					var nodePair = new KeyValuePair<
						GasNodeAbstract,GasNodeAbstract> (
							nodeInRow, nodeInColumn);

					var nodeInRowAdapter = nodeInRow.adapterForMatrixConstruction ();
					var nodeInColumnAdapter = nodeInColumn.adapterForMatrixConstruction ();

					double value = nodeInRowAdapter.matrixValueRespect (
						nodeInColumnAdapter,
						new MatrixComputationDataProviderDictionaryImplementation (
							LittleK, KvectorAtCurrentStep)
					);

					matrixAtCurrentStep.Add (nodePair, value);

				}
			}


			return null;
		}

		protected Dictionary<KeyValuePair<GasNodeAbstract,GasNodeAbstract>, Double> 
			computeKvector (
			Dictionary<GasNodeAbstract, double> unknowns,
			Dictionary<GasEdge, Double> Qvector)
		{
			return null;
		}

	}
}

