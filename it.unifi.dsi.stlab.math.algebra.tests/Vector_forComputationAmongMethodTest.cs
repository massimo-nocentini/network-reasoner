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
			Boolean closureCalledForAnIndex = false;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anIndex, 0, fromVector => {
				closureCalledForAnIndex = true;
				return fromVector;}
			)
			);

			var vectorForComputation = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { aDouble });

			Assert.AreEqual (expectedVector, vectorForComputation);
			Assert.IsTrue (closureCalledForAnIndex);
		}

		[Test()]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void inserting_a_single_value_with_wrong_order_and_asking_for_context_of_only_one_index_should_raise_error ()
		{
			var aDouble = 387.291;
			var anIndex = new Object ();
			Boolean closureCalledForAnIndex = false;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anIndex, 1, fromVector => {
				closureCalledForAnIndex = true;
				return fromVector;}
			)
			);

			var vectorForComputation = aVector.forComputationAmong (aList, 0);


		}

		[Test()]
		public void inserting_two_values_and_asking_for_rich_context_should_return_a_big_vector ()
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

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anIndex, 0, fromVector => {
				closureCalledForAnIndex = true;
				return fromVector;}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 1, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anotherIndex, 2, fromVector => {
				closureCalledForAnotherIndex = true;
				return fromVector;}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 3, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);

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

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anIndex, 0, fromVector => {
				closureCalledForAnIndex = true;
				return fromVector;}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 1, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anotherIndex, 2, fromVector => {
				closureCalledForAnotherIndex = true;
				return fromVector;}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 4, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);

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
			Boolean closureCalledForAnIndex = false;

			var anotherDouble = 23.291;
			var anotherIndex = new Object ();
			Boolean closureCalledForAnotherIndex = false;

			var defaultForOthers = 1;

			var aVector = new Vector<Object> ();
			aVector.atPut (anIndex, aDouble);
			aVector.atPut (anotherIndex, anotherDouble);

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				anIndex, 0, fromVector => {
				closureCalledForAnIndex = true;
				return fromVector;}
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 1, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);
			// here we do not add the anotherIndex object to the context in order
			// to produce a contradiction.
//			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
//				anotherIndex, 2, fromVector => {
//				closureCalledForAnotherIndex = true;
//				return fromVector;}
//			)
//			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				new object (), 2, fromVector => {
				throw new ClosureCallbackNotExpectedException ();}
			)
			);

			var anIndexIsntCoveredException = Assert.Catch<
				Vector<Object>.IndexNotCoveredByContextException> (
				() => aVector.forComputationAmong (aList, defaultForOthers));

			Assert.AreSame (anotherIndex, anIndexIsntCoveredException.IndexNotCovered);


			
		}

	}
}

