using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public abstract class ConditionChecker
	{
		public abstract void ensureBijectionOnVectors<IndexType> (
			Dictionary<IndexType, double>.KeyCollection keysInLeftVector, 
			Dictionary<IndexType, double>.KeyCollection keysInRightVector);

		public abstract void ensureVectorIsCoveredBy<IndexType> (
			Dictionary<IndexType, double>.KeyCollection keysInVector, List<IndexType> coveredKeys);

	}
}

