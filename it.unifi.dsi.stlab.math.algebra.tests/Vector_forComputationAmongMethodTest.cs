using System;
using NUnit.Framework;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class Vector_forComputationAmongMethodTest
	{
		[Test()]
		public void inserting_a_single_value_and_asking_for_context_of_only_one_index_should_return_a_vector_of_length_one ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);

			var aList = new Dictionary<Object, int> ();
			aList.Add (anIndex, 0);

			var vectorForComputation = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { aDouble });

			Assert.AreEqual (expectedVector, vectorForComputation);
		}

		[Test()]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void inserting_a_single_value_with_wrong_order_and_asking_for_context_of_only_one_index_should_raise_error ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);

			var aList = new Dictionary<Object, int> ();
			aList.Add (anIndex, 1);

			var vectorForComputation = aVector.forComputationAmong (aList, 0);


		}

		[Test()]
		public void inserting_two_values_and_asking_for_rich_context_should_return_a_big_vector ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();

			var anotherDouble = 23.291;
			var anotherIndex = new Object ();

			var defaultForOthers = 1;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);
			aVector.atPut (anotherIndex, anotherDouble);

			var aList = new Dictionary<Object, int> ();
			aList.Add (anIndex, 0);
			aList.Add (new object (), 1);
			aList.Add (anotherIndex, 2);
			aList.Add (new object (), 3);

			var vectorForComputation = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				aDouble,
				defaultForOthers,
				anotherDouble,
				defaultForOthers
			}
			);

			Assert.AreEqual (expectedVector, vectorForComputation);
			
		}

		[Test()]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void inserting_two_values_and_asking_for_rich_context_but_with_wrong_ordering_should_raise_error ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();
			Boolean closureCalledForAnIndex = false;

			var anotherDouble = 23.291;
			var anotherIndex = new Object ();
			Boolean closureCalledForAnotherIndex = false;

			var defaultForOthers = 1;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);
			aVector.atPut (anotherIndex, anotherDouble);

			var aList = new Dictionary<Object, int> ();
			aList.Add (anIndex, 0);
			aList.Add (new object (), 1);
			aList.Add (anotherIndex, 2);
			aList.Add (new object (), 4);

			var vectorForComputation = aVector.forComputationAmong (aList, defaultForOthers);

			var expectedVector = new DenseVector (new[] {
				aDouble,
				defaultForOthers,
				anotherDouble,
				defaultForOthers
			}
			);

			Assert.AreEqual (expectedVector, vectorForComputation);
			Assert.IsTrue (closureCalledForAnIndex);
			Assert.IsTrue (closureCalledForAnotherIndex);
			
		}

		[Test()]
		public void inserting_two_values_and_asking_for_rich_context_but_not_cover_all_indices_should_raise_error ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();

			var anotherDouble = 23.291;
			var anotherIndex = new Object ();

			var defaultForOthers = 1;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);
			aVector.atPut (anotherIndex, anotherDouble);

			var aList = new Dictionary<Object, int> ();
			aList.Add (anIndex, 0);
			aList.Add (new object (), 1);
			// here we do not add the anotherIndex object to the context in order
			// to produce a contradiction.
//			aList.Add (anotherIndex, 2);
			aList.Add (new object (), 2);

			var anIndexIsntCoveredException = Assert.Catch<
				IndexNotCoveredByContextException<Object>> (
				() => aVector.forComputationAmong (aList, defaultForOthers));

			Assert.AreSame (anotherIndex, anIndexIsntCoveredException.IndexNotCovered);


			
		}

	}
}

