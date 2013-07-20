using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Matrix<RowIndexType, ColumnIndexType, VType>
	{
		Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, VType> aMatrix{ get; set; }

		Dictionary<RowIndexType, SortedSet<KeyValuePair<RowIndexType, ColumnIndexType>>> keysByRowIndex{ get; set; }

		public Matrix ()
		{
			aMatrix = new Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, VType> ();
			keysByRowIndex = new Dictionary<RowIndexType, SortedSet<KeyValuePair<RowIndexType, ColumnIndexType>>> ();
		}

		void memoizeKeyByRowIndex (
			RowIndexType row, 
			KeyValuePair<RowIndexType, ColumnIndexType> key)
		{
			if (keysByRowIndex.ContainsKey (row) == false) {
				keysByRowIndex.Add (row, new SortedSet<KeyValuePair<RowIndexType, ColumnIndexType>> ());
			}

			keysByRowIndex [row].Add (key);
		}

		void mutateMatrix (
			KeyValuePair<RowIndexType, ColumnIndexType> key,
			Func<VType, VType> aSetBlock, 
			VType initialValue)
		{
			if (aMatrix.ContainsKey (key) == false) {
				aMatrix.Add (key, initialValue);
			}
			this.aMatrix [key] = aSetBlock.Invoke (aMatrix [key]);
		}

		public void atRowAtColumnPut (
			RowIndexType row,
			ColumnIndexType column, 
			Func<VType, VType> aSetBlock, 
			VType initialValue)
		{
			var key = new KeyValuePair<RowIndexType, ColumnIndexType> (row, column);

			mutateMatrix (key, aSetBlock, initialValue);

			memoizeKeyByRowIndex (row, key); 
		}

		public void doOnRowOf (
			RowIndexType rowIndex, 
			Func<ColumnIndexType, VType, VType> anUpdateBlock)
		{
			foreach (var matrixKey in keysByRowIndex[rowIndex]) {

				var updatedValue = anUpdateBlock.Invoke (
					matrixKey.Value, this.aMatrix [matrixKey]);

				this.aMatrix [matrixKey] = updatedValue;
			}
		}

		public Vector<RowIndexType, VType> rightProduct (
			Vector<RowIndexType, VType> aVector)
		{
			throw new NotImplementedException ();
		}

		public Vector<RowIndexType, double> Solve (
			Vector<RowIndexType, double> aVector)
		{
			throw new NotImplementedException ();
		}


	}
}

