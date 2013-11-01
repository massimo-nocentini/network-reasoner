using System;
using System.Collections.Generic;
using System.Linq;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class EnsureBijectionOnVectors<IndexType> : AlgebraConstraint
	{

		public Dictionary<IndexType, double>.KeyCollection KeysInLeftVector { get; set; }

		public Dictionary<IndexType, double>.KeyCollection KeysInRightVector{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.AlgebraConstraint
		public override void check ()
		{
			foreach (var keyInLeftVector in KeysInLeftVector) {
				if (KeysInRightVector.Contains (keyInLeftVector) == false) {
					throw new RightVectorHasMissingIndexException<IndexType>{ 
						MissingIndex = keyInLeftVector};
				}
			}

			foreach (var keyInRightVector in KeysInRightVector) {
				if (KeysInLeftVector.Contains (keyInRightVector) == false) {
					throw new LeftVectorHasMissingIndexException<IndexType>{ 
						MissingIndex = keyInRightVector};
				}
			}
		}
		#endregion

	}
}

