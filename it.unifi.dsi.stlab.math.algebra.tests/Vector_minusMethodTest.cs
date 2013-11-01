using System;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class Vector_minusMethodTest
	{
		[Test()]
		public void minus_operation_on_two_vectors_with_equal_key_set_should_produce_a_valid_exception ()
		{
			var aFirstDoubleLeft = 45;
			var aFirstDoubleRight = 40;
			var aFirstIndex = new Object ();

			var aSecondDoubleLeft = 3.5;
			var aSecondDoubleRight = 1;	
			var aSecondIndex = new Object ();

			var aThirdDoubleLeft = 0;
			var aThirdDoubleRight = 1.5;
			var aThirdIndex = new Object ();

			var aVectorLeft = new Vector<Object> ();
			aVectorLeft.atPut (aFirstIndex, aFirstDoubleLeft);
			aVectorLeft.atPut (aSecondIndex, aSecondDoubleLeft);
			aVectorLeft.atPut (aThirdIndex, aThirdDoubleLeft);

			var aVectorRight = new Vector<Object> ();
			aVectorRight.atPut (aFirstIndex, aFirstDoubleRight);
			aVectorRight.atPut (aSecondIndex, aSecondDoubleRight);
			aVectorRight.atPut (aThirdIndex, aThirdDoubleRight);

			var aList = new Dictionary<Object, int> ();
			aList.Add (aFirstIndex, 0);
			aList.Add (aSecondIndex, 1);
			aList.Add (aThirdIndex, 2);

			var leftMinusRightVector = aVectorLeft.minus (aVectorRight)
				.forComputationAmong (aList, new ValueHolderCarryInfo<Double>{Value = 0});

			var expectedVector = new DenseVector (new []{
				aFirstDoubleLeft - aFirstDoubleRight,
				aSecondDoubleLeft - aSecondDoubleRight,
				aThirdDoubleLeft - aThirdDoubleRight
			}
			);

			Assert.AreEqual (expectedVector, leftMinusRightVector);
			
		}

		[Test()]
		public void a_missing_key_in_right_vector_should_raise_error ()
		{
			var aFirstDoubleLeft = 45;
			var aFirstDoubleRight = 40;
			var aFirstIndex = new Object ();

			var aSecondDoubleLeft = 3.5;
			var aSecondDoubleRight = 1;	
			var aSecondIndex = new Object ();

			var aThirdDoubleLeft = 0;
			var aThirdIndex = new Object ();

			var aVectorLeft = new Vector<Object> ();
			aVectorLeft.atPut (aFirstIndex, aFirstDoubleLeft);
			aVectorLeft.atPut (aSecondIndex, aSecondDoubleLeft);
			aVectorLeft.atPut (aThirdIndex, aThirdDoubleLeft);

			// here we do not put the third index in order to get an error.
			var aVectorRight = new Vector<Object> ();
			aVectorRight.atPut (aFirstIndex, aFirstDoubleRight);
			aVectorRight.atPut (aSecondIndex, aSecondDoubleRight);

			var aList = new Dictionary<Object, int> ();
			aList.Add (aFirstIndex, 0);
			aList.Add (aSecondIndex, 1);
			aList.Add (aThirdIndex, 2);

			var expectedException = Assert.Catch<RightVectorHasMissingIndexException<Object>> (
				() => aVectorLeft.minus (aVectorRight)
						.forComputationAmong (aList, 
			                      new ValueHolderCarryInfo<Double>{Value = 0})
			);

			Assert.AreSame (aThirdIndex, expectedException.MissingIndex);
			
		}

		[Test()]
		public void a_missing_key_in_left_vector_should_raise_error ()
		{
			var aFirstDoubleLeft = 45;
			var aFirstDoubleRight = 40;
			var aFirstIndex = new Object ();

			var aSecondDoubleLeft = 3.5;
			var aSecondDoubleRight = 1;	
			var aSecondIndex = new Object ();

			var aThirdDoubleRight = 1.5;
			var aThirdIndex = new Object ();

			// here we do not put the third index in order to get an error.
			var aVectorLeft = new Vector<Object> ();
			aVectorLeft.atPut (aFirstIndex, aFirstDoubleLeft);
			aVectorLeft.atPut (aSecondIndex, aSecondDoubleLeft);

			var aVectorRight = new Vector<Object> ();
			aVectorRight.atPut (aFirstIndex, aFirstDoubleRight);
			aVectorRight.atPut (aSecondIndex, aSecondDoubleRight);
			aVectorRight.atPut (aThirdIndex, aThirdDoubleRight);

			var aList = new Dictionary<Object, int> ();
			aList.Add (aFirstIndex, 0);
			aList.Add (aSecondIndex, 1);
			aList.Add (aThirdIndex, 2);

			var expectedException = Assert.Catch<LeftVectorHasMissingIndexException<Object>> (
				() => aVectorLeft.minus (aVectorRight)
				.forComputationAmong (aList, new ValueHolderCarryInfo<Double>{Value = 0})
			);

			Assert.AreSame (aThirdIndex, expectedException.MissingIndex);
			
		}

		// TODO: check the minus between two empty vectors.
	}
}

