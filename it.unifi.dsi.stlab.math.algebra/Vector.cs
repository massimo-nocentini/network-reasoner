using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType>
	{

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
			var keys = aVector.Keys;
			foreach (var key in keys) {
				aVector [key] = updater.Invoke (key, aVector [key]);
			}
		}

		public Vector forComputationAmong (
			List<Tuple<IndexType, int, Func<Double, double>>> someIndices, 
			double defaultForMissingIndices)
		{
			List<Tuple<int, double>> orderedEnumerable = new List<Tuple<int, double>> ();

			List<IndexType> coveredIndices = new List<IndexType> ();

			someIndices.ForEach (aTuple => {

				if (aVector.ContainsKey (aTuple.Item1)) {
					orderedEnumerable.Add (new Tuple<int, double> (
						aTuple.Item2, aTuple.Item3.Invoke (aVector [aTuple.Item1]))
					);

					coveredIndices.Add (aTuple.Item1);
				} else {
					orderedEnumerable.Add (new Tuple<int, double> (
						aTuple.Item2, defaultForMissingIndices)
					);
				}
			}
			);

			this.aVector.Keys.ToList ().ForEach (
				aKey => {
				if (coveredIndices.Contains (aKey) == false) {
					throw new IndexNotCoveredByContextException{ IndexNotCovered = aKey};
				}}
			);


			return DenseVector.OfIndexedEnumerable (orderedEnumerable.Count, orderedEnumerable);
		}
	}
}

