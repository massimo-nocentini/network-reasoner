using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers.StopCriterium;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Double.Solvers.Iterative;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Matrix<RowIndexType, ColumnIndexType>
	{
		Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, Double> aMatrix{ get; set; }

		Dictionary<RowIndexType, HashSet<KeyValuePair<RowIndexType, ColumnIndexType>>> keysByRowIndex{ get; set; }

		HashSet<ColumnIndexType> ColumnIndices { get; set; }

		HashSet<RowIndexType> RowIndices { get; set; }

		public Matrix ()
		{
			aMatrix = new Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, Double> ();
			keysByRowIndex = new Dictionary<RowIndexType, HashSet<KeyValuePair<RowIndexType, ColumnIndexType>>> ();
			RowIndices = new HashSet<RowIndexType> ();
			ColumnIndices = new HashSet<ColumnIndexType> ();
		}

		void memoizeKeyByRowIndex (
			RowIndexType row, 
			KeyValuePair<RowIndexType, ColumnIndexType> key)
		{
			if (keysByRowIndex.ContainsKey (row) == false) {
				keysByRowIndex.Add (
					row, new HashSet<KeyValuePair<RowIndexType, ColumnIndexType>> ());
			}

			keysByRowIndex [row].Add (key);
		}

		void mutateMatrix (
			KeyValuePair<RowIndexType, ColumnIndexType> key,
			Func<Double, Double> aSetBlock, 
			Double initialValue)
		{
			if (aMatrix.ContainsKey (key) == false) {
				aMatrix.Add (key, initialValue);
			}
			this.aMatrix [key] = aSetBlock.Invoke (aMatrix [key]);
		}

		public void atRowAtColumnPut (
			RowIndexType row,
			ColumnIndexType column, 
			Func<Double, Double> aSetBlock, 
			Double initialValue)
		{
			var key = new KeyValuePair<RowIndexType, ColumnIndexType> (row, column);
			this.RowIndices.Add (row);
			this.ColumnIndices.Add (column);

			mutateMatrix (key, aSetBlock, initialValue);

			memoizeKeyByRowIndex (row, key); 
		}

		public void doOnRowOf (
			RowIndexType rowIndex, 
			Func<ColumnIndexType, Double, Double> anUpdateBlock)
		{
			foreach (var matrixKey in keysByRowIndex[rowIndex]) {

				var updatedValue = anUpdateBlock.Invoke (
					matrixKey.Value, this.aMatrix [matrixKey]);

				this.aMatrix [matrixKey] = updatedValue;
			}
		}

		public Vector<RowIndexType> rightProduct (
			Vector<ColumnIndexType> aVector)
		{
			Vector<RowIndexType> result =
				new Vector<RowIndexType> ();

			foreach (var rowKey in this.keysByRowIndex.Keys) {
				var valueForRowKeyResultVectorComponent = 0.0;
				foreach (var matrixIndex in this.keysByRowIndex[rowKey]) {
					valueForRowKeyResultVectorComponent +=
						aVector.valueAt (matrixIndex.Value) * this.aMatrix [matrixIndex];
				}
				result.atPut (rowKey, valueForRowKeyResultVectorComponent);
			}

			return result;
		}

		public Vector<RowIndexType> Solve (
			Vector<RowIndexType> aVector)
		{

			List<Tuple<int, int, double>> indices = 
				new List<Tuple<int, int, double>> ();

			Dictionary<RowIndexType, int> rowsEnumeration = 
				new Dictionary<RowIndexType, int> ();

			Dictionary<ColumnIndexType, int> columnsEnumeration = 
				new Dictionary<ColumnIndexType, int> ();

			Dictionary<int, RowIndexType> coefficientsEnumeration = 
				new Dictionary<int, RowIndexType> ();

			var aListOfIndicesForCoefficientVector = 
				new List<Tuple<RowIndexType, int>> ();

			int rowIntIndex = 0; 
			foreach (var rowIndex in this.RowIndices) {
				rowsEnumeration.Add (rowIndex, rowIntIndex);
				coefficientsEnumeration.Add (rowIntIndex, rowIndex);
				aListOfIndicesForCoefficientVector.Add (
					new Tuple<RowIndexType, int> (rowIndex, rowIntIndex));

				rowIntIndex = rowIntIndex + 1;
			}

			int columnIntIndex = 0; 
			foreach (var columnIndex in this.ColumnIndices) {
				columnsEnumeration.Add (columnIndex, columnIntIndex);
				columnIntIndex = columnIntIndex + 1;
			}

			foreach (var matrixIndex in this.aMatrix.Keys) {
				indices.Add (new Tuple<int, int, double> (
						rowsEnumeration [matrixIndex.Key],
						columnsEnumeration [matrixIndex.Value],
						this.aMatrix [matrixIndex])
				);
			}

			var aMatrixForSolving = SparseMatrix.OfIndexed (
				rowsEnumeration.Count,
				columnsEnumeration.Count,
				indices);

			var aVectorForSolving = aVector.forComputationAmong (
				aListOfIndicesForCoefficientVector, 0);
			
			// Stop calculation if 1000 iterations reached during calculation
			var iterationCountStopCriterium = new IterationCountStopCriterium (100000000);

			// Stop calculation if residuals are below 1E-10 --> the calculation is considered converged
			var residualStopCriterium = new ResidualStopCriterium (1e-10);
 
			// Create monitor with defined stop criteriums
			var monitor = new Iterator (new IIterationStopCriterium[] {
				iterationCountStopCriterium,
				residualStopCriterium
			}
			);

			// Create Bi-Conjugate Gradient Stabilized solver
			var solver = new BiCgStab (monitor);

			// 1. Solve the matrix equation
			var resultX = solver.Solve (aMatrixForSolving, aVectorForSolving);

			Vector<RowIndexType> result = new Vector<RowIndexType> ();
			for (int i = 0; i < resultX.Count; i = i + 1) {
				result.atPut (coefficientsEnumeration [i], resultX [i]);
			}

			return result;
		}

		public Matrix forComputation ()
		{
			return null;
		}
	}
}

