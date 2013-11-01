using System;
using System.Linq;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class EnsureVectorIsCoveredByKeySet<IndexType> : AlgebraConstraint
	{
		public Dictionary<IndexType, double>.KeyCollection VectorKeys {
			get;
			set;
		}

		public List<IndexType> CoveringKeys {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.AlgebraConstraint
		public override void check ()
		{
			VectorKeys.ToList ().ForEach (
				aKey => {
				if (CoveringKeys.Contains (aKey) == false) {
					throw new IndexNotCoveredByContextException<IndexType>{ 
						IndexNotCovered = aKey};
				}}
			);
		}
		#endregion

	}
}

