using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class ConditionCheckerEnabled : it.unifi.dsi.stlab.math.algebra.ConditionChecker
	{
		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.ConditionChecker
		public override void ensureBijectionOnVectors<IndexType> (
			Dictionary<IndexType, double>.KeyCollection keysInLeftVector, 
			Dictionary<IndexType, double>.KeyCollection keysInRightVector)
		{
			
			foreach (var keyInLeftVector in keysInLeftVector) {
				if (keysInRightVector.Contains (keyInLeftVector) == false) {
					throw new RightVectorHasMissingIndexException<IndexType>{ 
						MissingIndex = keyInLeftVector};
				}
			}

			foreach (var keyInRightVector in keysInRightVector) {
				if (keysInLeftVector.Contains (keyInRightVector) == false) {
					throw new LeftVectorHasMissingIndexException<IndexType>{ 
						MissingIndex = keyInRightVector};
				}
			}

		}

		public override void ensureVectorIsCoveredBy<IndexType> (
			Dictionary<IndexType, double>.KeyCollection keysInVector, 
			List<IndexType> coveredKeys)
		{			
			keysInVector.ToList ().ForEach (
				aKey => {
				if (coveredKeys.Contains (aKey) == false) {
					throw new IndexNotCoveredByContextException<IndexType>{ 
						IndexNotCovered = aKey};
				}}
			);
		}
		#endregion
	}

}

