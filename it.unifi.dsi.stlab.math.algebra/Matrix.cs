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

			Dictionary<RowIndexType, int> rowsEnumeration =
				this.enumerate (this.RowIndices);

			Dictionary<ColumnIndexType, int> columnsEnumeration = 
				this.enumerate (this.ColumnIndices);

			return this.SolveWithGivenEnumerations (
				rowsEnumeration, columnsEnumeration, aVector);

		}

		protected virtual Dictionary<T, int> enumerate<T> (
			HashSet<T> aSet)
		{
			Dictionary<T, int> anEnumeration = 
				new Dictionary<T, int> ();

			int anIndex = 0; 
			foreach (var anElement in aSet) {
				anEnumeration.Add (anElement, anIndex);
				anIndex = anIndex + 1;
			}

			return anEnumeration;
		}

		public Vector<RowIndexType> SolveWithGivenEnumerations (
			Dictionary<RowIndexType, int> rowsEnumeration,
			Dictionary<ColumnIndexType, int> columnsEnumeration,
			Vector<RowIndexType> aVector)
		{
			List<Tuple<int, int, double>> indices = 
				new List<Tuple<int, int, double>> ();

			Dictionary<int, RowIndexType> coefficientsInverseEnumeration = 
				new Dictionary<int, RowIndexType> ();

			foreach (var pair in rowsEnumeration) {
				coefficientsInverseEnumeration.Add (pair.Value, pair.Key);
			}

			foreach (var matrixIndex in this.aMatrix.Keys) {
				indices.Add (new Tuple<int, int, double> (
						rowsEnumeration [matrixIndex.Key],
						columnsEnumeration [matrixIndex.Value],
						this.aMatrix [matrixIndex])
				);
			}

			var aMatrixForSolving = DenseMatrix.OfIndexed (
				rowsEnumeration.Count,
				columnsEnumeration.Count,
				indices);

			var aVectorForSolving = aVector.forComputationAmong (
				rowsEnumeration, 0);

			var resultX = aMatrixForSolving.LU ().Solve (aVectorForSolving);

			Vector<RowIndexType> result = new Vector<RowIndexType> ();
			for (int i = 0; i < resultX.Count; i = i + 1) {
				result.atPut (coefficientsInverseEnumeration [i], resultX [i]);
			}

			return result;
		}
	}
}

