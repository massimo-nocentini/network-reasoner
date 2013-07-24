using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Matrix<RowIndexType, ColumnIndexType>
	{
		Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, Double> aMatrix{ get; set; }

		Dictionary<RowIndexType, HashSet<KeyValuePair<RowIndexType, ColumnIndexType>>> keysByRowIndex{ get; set; }

		public Matrix ()
		{
			aMatrix = new Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, Double> ();
			keysByRowIndex = new Dictionary<RowIndexType, HashSet<KeyValuePair<RowIndexType, ColumnIndexType>>> ();
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
			throw new NotImplementedException ();
		}


	}
}

