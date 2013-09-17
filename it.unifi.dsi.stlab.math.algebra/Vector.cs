using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType>
	{
		public abstract class MissingKeyAbstractException : Exception
		{
			public IndexType MissingIndex{ get; set; }
		}

		public class RightVectorHasMissingIndexException:MissingKeyAbstractException
		{

		}

		public class LeftVectorHasMissingIndexException:MissingKeyAbstractException
		{

		}

		/**
		 *	Defining here this exception isn't correct because we're mixing
		 *	some precondition-checking logic with the logic of the vector. 
		 *	As always we should delegate to another hierarchy to do the check 
		 *	for us, in this way we can use a "false" checker that does nothing
		 *	at all, while we can implement a "full" checker that is really rigorous
		 *	to ensure required working conditions are satisfied.
		 */
		public class IndexNotCoveredByContextException : Exception
		{
			public IndexType IndexNotCovered{ get; set; }
		}

		Dictionary<IndexType, Double> aVector{ get; set; }

		public Vector ()
		{
			this.aVector = new Dictionary<IndexType, Double> ();
		}

		public Double valueAt (IndexType index)
		{
			return this.aVector [index];
		}

		public void atPut (IndexType index, Double aValue)
		{
			this.aVector.Add (index, aValue);
		}

		public Vector<IndexType> minus (
			Vector<IndexType> anotherVector)
		{
			foreach (var keyInLeftVector in this.aVector.Keys) {
				if (anotherVector.aVector.Keys.Contains (keyInLeftVector) == false) {
					throw new RightVectorHasMissingIndexException{ 
						MissingIndex = keyInLeftVector};
				}
			}

			foreach (var keyInRightVector in anotherVector.aVector.Keys) {
				if (this.aVector.Keys.Contains (keyInRightVector) == false) {
					throw new LeftVectorHasMissingIndexException{ 
						MissingIndex = keyInRightVector};
				}
			}

			Vector<IndexType> result = 
				new Vector<IndexType> ();

			foreach (IndexType key in this.aVector.Keys) {
				var valueForKeyInOtherVector = 
					anotherVector.valueAt (key);

				result.atPut (key, this.valueAt (key) - 
					valueForKeyInOtherVector
				);
			}
			return result;
		}

		public void updateEach (Func<IndexType, Double, Double> updater)
		{
			Dictionary<IndexType, Double> updated = 
				new Dictionary<IndexType, double> ();

			var keys = aVector.Keys;
			foreach (var key in keys) {
				var updatedValue = updater.Invoke (key, aVector [key]);
				updated.Add (key, updatedValue);
			}

			foreach (var pair in updated) {
				aVector [pair.Key] = pair.Value;
			}

		}

		public Vector forComputationAmong (
			Dictionary<IndexType, int> someIndices, 
			double defaultForMissingIndices)
		{
			List<Tuple<int, double>> orderedEnumerable = 
				new List<Tuple<int, double>> ();

			List<IndexType> coveredIndices = new List<IndexType> ();

			foreach (var pair in someIndices) {

				var index = pair.Key;
				var position = pair.Value;


				if (aVector.ContainsKey (index)) {
					var value = aVector [index];

					orderedEnumerable.Add (new Tuple<int, double> (
						position, value)
					);

					coveredIndices.Add (index);
				} else {
					orderedEnumerable.Add (new Tuple<int, double> (
						position, defaultForMissingIndices)
					);
				}
			}


			this.aVector.Keys.ToList ().ForEach (
				aKey => {
				if (coveredIndices.Contains (aKey) == false) {
					throw new IndexNotCoveredByContextException{ 
						IndexNotCovered = aKey};
				}}
			);


			return DenseVector.OfIndexedEnumerable (
				orderedEnumerable.Count, orderedEnumerable);
		}

		public Vector<IndexType> ratio (
			Vector<IndexType> anotherVector)
		{
			// factor out this code --------------------------------
			foreach (var keyInLeftVector in this.aVector.Keys) {
				if (anotherVector.aVector.Keys.Contains (keyInLeftVector) == false) {
					throw new RightVectorHasMissingIndexException{ 
						MissingIndex = keyInLeftVector};
				}
			}

			foreach (var keyInRightVector in anotherVector.aVector.Keys) {
				if (this.aVector.Keys.Contains (keyInRightVector) == false) {
					throw new LeftVectorHasMissingIndexException{ 
						MissingIndex = keyInRightVector};
				}
			}
			// -----------------------------------------------------

			Vector<IndexType> result = 
				new Vector<IndexType> ();

			foreach (IndexType key in this.aVector.Keys) {
				var valueForKeyInOtherVector = 
					anotherVector.valueAt (key);

				result.atPut (key, this.valueAt (key) /
					valueForKeyInOtherVector
				);
			}
			return result;
		}

		public bool TrueForAll (Predicate<IndexType> predicate)
		{
			return this.aVector.Keys.ToList ().TrueForAll (predicate);
		}


	}
}

