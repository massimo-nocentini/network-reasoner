using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class VectorShouldCovertColumnsSpaceForMatrixRightProduct<RowIndexType, ColumnIndexType> : AlgebraConstraint
	{
		public Dictionary<RowIndexType, HashSet<KeyValuePair<RowIndexType, ColumnIndexType>>> MatrixElementsPerRows {
			get;
			set;
		}

		public Vector<ColumnIndexType> RightVector {
			get;
			set;
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.AlgebraConstraint
		public override void check ()
		{
			foreach (var rowKey in MatrixElementsPerRows.Keys) {

				foreach (var matrixIndex in MatrixElementsPerRows[rowKey]) {
					if (RightVector.containsKey (matrixIndex.Value) == false) {
						throw new IndexNotCoveredByContextException<ColumnIndexType>{
							IndexNotCovered = matrixIndex.Value
						};
					}
						
				}
			}
		}
		#endregion


	}
}

