using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType>
	{
		Dictionary<IndexType, Double> aVector{ get; set; }

		ConditionChecker ConditionChecker { get; set; }

		public Vector ()
		{
			this.aVector = new Dictionary<IndexType, Double> ();
			this.ConditionChecker = new ConditionCheckerEnabled ();
		}

		public Double valueAt (IndexType index)
		{
			return this.aVector [index];
		}

		public void atPut (IndexType index, Double aValue)
		{
			this.aVector.Add (index, aValue);
		}

		public void updateEach (Func<IndexType, Double, Double> updater)
		{
			Dictionary<IndexType, Double> updated = 
				new Dictionary<IndexType, double> ();

			foreach (var key in aVector.Keys) {
				var updatedValue = updater.Invoke (key, this.valueAt (key));
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

			List<IndexType> coveredKeys = new List<IndexType> ();

			foreach (var pair in someIndices) {

				var index = pair.Key;
				var position = pair.Value;

				if (aVector.ContainsKey (index)) {

					var value = this.valueAt (index);

					orderedEnumerable.Add (new Tuple<int, double> (
						position, value)
					);

					coveredKeys.Add (index);
				} else {
					orderedEnumerable.Add (new Tuple<int, double> (
						position, defaultForMissingIndices)
					);
				}
			}

			this.ConditionChecker.ensureVectorIsCoveredBy (this.aVector.Keys, 
			                                               coveredKeys);

			return DenseVector.OfIndexedEnumerable (
				orderedEnumerable.Count, orderedEnumerable);
		}

		protected virtual Func<IndexType, double, double, double> doRatio ()
		{
			return (key, valueInLeftVector, valueInRightVector) => 
				valueInLeftVector / valueInRightVector;
		}

		protected virtual Func<IndexType, double, double, double> doSubtraction ()
		{
			return (key, valueInLeftVector, valueInRightVector) => 
				valueInLeftVector - valueInRightVector;
		}

		public Vector<IndexType> ratio (
			Vector<IndexType> anotherVector)
		{
			return inBijectionWithDo (anotherVector, doRatio ());
		}

		public Vector<IndexType> minus (
			Vector<IndexType> anotherVector)
		{
			return inBijectionWithDo (anotherVector, doSubtraction ());
		}

		public virtual Vector<IndexType> inBijectionWithDo (
			Vector<IndexType> anotherVector,
			Func<IndexType, double, double, double> onBijectionAction)
		{
			this.ConditionChecker.ensureBijectionOnVectors<IndexType> (
				this.aVector.Keys, anotherVector.aVector.Keys);

			Vector<IndexType> result = new Vector<IndexType> ();

			foreach (IndexType key in this.aVector.Keys) {

				result.atPut (key,
				              onBijectionAction.Invoke (key, 
				                   this.valueAt (key),
				                   anotherVector.valueAt (key))
				);
			}

			return result;
		}

		public bool atLeastOneSatisfy (Predicate<IndexType> predicate)
		{
			return this.aVector.Keys.ToList ().Any (
				key => predicate.Invoke (key));
		}

		public IndexType findKeyWithMinValue ()
		{
			if (this.aVector.Count == 0) {
				throw new Exception ("Impossible to find a key with " +
					"min value because there's no keys at all!"
				);
			}

			var min = Double.MaxValue;
			IndexType minKey = default(IndexType);
			
			foreach (IndexType key in this.aVector.Keys) {
				var currentValue = this.valueAt (key);
				if (currentValue < min) {
					minKey = key;
					min = currentValue;
				}
			}

			return minKey;
		}

	}
}

