using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Matrix<RowIndexType, ColumnIndexType, VType>
	{
		Dictionary<KeyValuePair<RowIndexType, ColumnIndexType>, VType> aMatrix;

		public void atRowAtColumnPut (RowIndexType row, ColumnIndexType column, double value)
		{
			throw new NotImplementedException ();
		}

		public void doOnRowOf (RowIndexType rowIndex, Func<ColumnIndexType, VType> anUpdateBlock)
		{
		}
	}
}

