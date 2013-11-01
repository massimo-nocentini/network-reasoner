using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;
using it.unifi.dsi.stlab.utilities.value_holders;

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

		void ensureVectorKeysAreCoveredBy (List<IndexType> coveringKeys)
		{
			var coveredConstraint = new EnsureVectorIsCoveredByKeySet<IndexType> ();
			coveredConstraint.VectorKeys = aVector.Keys;
			coveredConstraint.CoveringKeys = coveringKeys;
			ConditionChecker.ensure (coveredConstraint);
		}

		public Vector forComputationAmong (
			Dictionary<IndexType, int> someIndices, 
			ValueHolder<Double> defaultForMissingIndices)
		{
			List<Tuple<int, double>> orderedEnumerable = 
				new List<Tuple<int, double>> ();

			List<IndexType> coveringKeys = new List<IndexType> ();

			foreach (var pair in someIndices) {

				var index = pair.Key;
				var position = pair.Value;

				if (this.containsKey (index)) {

					var value = this.valueAt (index);

					orderedEnumerable.Add (new Tuple<int, double> (
						position, value)
					);

					coveringKeys.Add (index);
				} else {
					orderedEnumerable.Add (new Tuple<int, double> (
						position, defaultForMissingIndices.getValue ())
					);
				}
			}

			ensureVectorKeysAreCoveredBy (coveringKeys);

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
			this.ensureBijectionExistsWith (anotherVector);

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

		protected virtual void ensureBijectionExistsWith (
			Vector<IndexType> anotherVector)
		{
			var bijectionConstraint = new EnsureBijectionOnVectors<IndexType> ();
			bijectionConstraint.KeysInLeftVector = aVector.Keys;
			bijectionConstraint.KeysInRightVector = anotherVector.aVector.Keys;

			this.ConditionChecker.ensure (bijectionConstraint);
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

		public bool containsKey (IndexType key)
		{
			return aVector.ContainsKey (key);
		}


	}
}

