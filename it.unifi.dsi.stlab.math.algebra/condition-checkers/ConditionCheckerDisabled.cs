using System;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class ConditionCheckerDisabled : ConditionChecker
	{
		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.ConditionChecker
		public override void ensureBijectionOnVectors<IndexType> (System.Collections.Generic.Dictionary<IndexType, double>.KeyCollection keysInLeftVector, System.Collections.Generic.Dictionary<IndexType, double>.KeyCollection keysInRightVector)
		{
		}

		public override void ensureVectorIsCoveredBy<IndexType> (System.Collections.Generic.Dictionary<IndexType, double>.KeyCollection keysInVector, System.Collections.Generic.List<IndexType> coveredKeys)
		{
		}
		#endregion
	}
}

