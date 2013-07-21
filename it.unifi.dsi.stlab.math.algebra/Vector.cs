using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType, VType>
	{
		public class IndexNotCoveredByContextException : Exception
		{
			public IndexType IndexNotCovered{ get; set; }
		}

		Dictionary<IndexType, VType> aVector{ get; set; }

		public Vector ()
		{
			this.aVector = new Dictionary<IndexType, VType> ();
		}

		public VType valueAt (IndexType index)
		{
			return this.aVector [index];
		}

		public void atPut (IndexType index, VType aValue)
		{
			this.aVector.Add (index, aValue);
		}

		public Vector<IndexType, VType> minus (
			Vector<IndexType, VType> aVector)
		{
			throw new NotImplementedException ();
		}

		public void updateEach (Func<IndexType, VType, VType> updater)
		{
			var keys = aVector.Keys;
			foreach (var key in keys) {
				aVector [key] = updater.Invoke (key, aVector [key]);
			}
		}

		public Vector forComputationAmong (
			List<Tuple<IndexType, int, Func<VType, double>>> someIndices, 
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

