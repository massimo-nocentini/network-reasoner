using System;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class Matrix_rightProductMethodTest
	{
		[Test()]
		public void identity_matrix_times_a_vector_should_return_that_vector ()
		{
			Matrix<Object, String> anIdentityMatrix = 
				new Matrix<object, string> ();

			Object firstRowIndex = new object ();
			Object secondRowIndex = new object ();
			Object thirdRowIndex = new object ();

			String firstColumnIndex = "first column";
			String secondColumnIndex = "second column";
			String thirdColumnIndex = "third column";

			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  firstColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  1);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  1);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  1);

			Vector<String> aVector = new Vector<string> ();
			aVector.atPut (firstColumnIndex, 3);
			aVector.atPut (secondColumnIndex, 4);
			aVector.atPut (thirdColumnIndex, 5);

			var aList = new Dictionary<object, int> ();
			aList.Add (firstRowIndex, 0);
			aList.Add (secondRowIndex, 1);
			aList.Add (thirdRowIndex, 2);

			var rightProductVector = anIdentityMatrix.rightProduct (aVector);
			var rightProductVectorForComputation = 
				rightProductVector.forComputationAmong (
					aList, new ValueHolderCarryInfo<Double>{Value = 0});

			Vector expectedRightProductVector = 
				new DenseVector (new []{3.0,4.0,5.0});
		
			Assert.AreNotSame (expectedRightProductVector, aVector);
			Assert.AreEqual (expectedRightProductVector, rightProductVectorForComputation);
			

		}

		[Test()]
		public void a_normal_right_product ()
		{
			Matrix<Object, String> anIdentityMatrix = 
				new Matrix<object, string> ();

			Object firstRowIndex = new object ();
			Object secondRowIndex = new object ();
			Object thirdRowIndex = new object ();

			String firstColumnIndex = "first column";
			String secondColumnIndex = "second column";
			String thirdColumnIndex = "third column";

			Double firstRowSecondColumnValue = 3;
			Double firstRowThirdColumnValue = 1;
			Double secondRowFirstColumnValue = 1;
			Double secondRowSecondColumnValue = -1;
			Double thirdRowSecondColumnValue = 4;
			Double thirdRowThirdColumnValue = -2;

			Double firstVectorComponent = 2;
			Double secondVectorComponent = 1;
			Double thirdVectorComponent = 3;
			
			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  firstRowSecondColumnValue);

			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  firstRowThirdColumnValue);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  firstColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  secondRowFirstColumnValue);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  secondRowSecondColumnValue);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  thirdRowSecondColumnValue);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  thirdRowThirdColumnValue);

			Vector<String> aVector = new Vector<string> ();
			aVector.atPut (firstColumnIndex, firstVectorComponent);
			aVector.atPut (secondColumnIndex, secondVectorComponent);
			aVector.atPut (thirdColumnIndex, thirdVectorComponent);

			var aList = new Dictionary<object, int> ();
			aList.Add (firstRowIndex, 0);
			aList.Add (secondRowIndex, 1);
			aList.Add (thirdRowIndex, 2);

			var rightProductVector = anIdentityMatrix.rightProduct (aVector);
			var rightProductVectorForComputation = 
				rightProductVector.forComputationAmong (
					aList, new ValueHolderCarryInfo<double>{Value = 0});

			Vector expectedRightProductVector = 
				new DenseVector (new []{
					secondVectorComponent * firstRowSecondColumnValue + thirdVectorComponent * firstRowThirdColumnValue,
					firstVectorComponent * secondRowFirstColumnValue + secondVectorComponent * secondRowSecondColumnValue,
					secondVectorComponent * thirdRowSecondColumnValue + thirdVectorComponent * thirdRowThirdColumnValue,
				}
			);
		
			Assert.AreNotSame (expectedRightProductVector, aVector);
			Assert.AreEqual (expectedRightProductVector, rightProductVectorForComputation);
			

		}

		[Test()]
		public void a_matrix_dot_zero_vector_should_produce_the_zero_vector ()
		{
			Matrix<Object, String> anIdentityMatrix = 
				new Matrix<object, string> ();

			Object firstRowIndex = new object ();
			Object secondRowIndex = new object ();
			Object thirdRowIndex = new object ();

			String firstColumnIndex = "first column";
			String secondColumnIndex = "second column";
			String thirdColumnIndex = "third column";
 
			Double zeroValue = 0;

			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  firstColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  10);

			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  20);

			anIdentityMatrix.atRowAtColumnPut (firstRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  30);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  firstColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  40);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  50);

			anIdentityMatrix.atRowAtColumnPut (secondRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  60);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  firstColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  70);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  secondColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  80);

			anIdentityMatrix.atRowAtColumnPut (thirdRowIndex, 
			                                  thirdColumnIndex,
			                                  aCumulate => aCumulate, 
			                                  90);

			Vector<String> aVector = new Vector<string> ();
			aVector.atPut (firstColumnIndex, zeroValue);
			aVector.atPut (secondColumnIndex, zeroValue);
			aVector.atPut (thirdColumnIndex, zeroValue);

			var aList = new Dictionary<object, int> ();
			aList.Add (firstRowIndex, 0);
			aList.Add (secondRowIndex, 1);
			aList.Add (thirdRowIndex, 2);

			var rightProductVector = anIdentityMatrix.rightProduct (aVector);
			var rightProductVectorForComputation = 
				rightProductVector.forComputationAmong (
					aList, new ValueHolderCarryInfo<double>{Value = 0});

			Vector expectedRightProductVector = 
				new DenseVector (new []{
					zeroValue,zeroValue,zeroValue
				}
			);
		
			Assert.AreNotSame (expectedRightProductVector, aVector);
			Assert.AreEqual (expectedRightProductVector, rightProductVectorForComputation);
			

		}
	}
}

