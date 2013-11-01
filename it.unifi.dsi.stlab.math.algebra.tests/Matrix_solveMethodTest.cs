using System;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.value_holders;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class Matrix_solveMethodTest
	{
		[Test()]
		public void solving_a_system ()
		{
			Matrix<Object, String> aMatrix = 
				new Matrix<object, string> ();

			Object firstRowIndex = new object ();
			Object secondRowIndex = new object ();
			Object thirdRowIndex = new object ();

			String firstColumnIndex = "first column";
			String secondColumnIndex = "second column";
			String thirdColumnIndex = "third column";

			aMatrix.atRowAtColumnPut (firstRowIndex, firstColumnIndex, cumulate => cumulate, 5);
			aMatrix.atRowAtColumnPut (firstRowIndex, secondColumnIndex, cumulate => cumulate, 2);
			aMatrix.atRowAtColumnPut (firstRowIndex, thirdColumnIndex, cumulate => cumulate, -4);
			aMatrix.atRowAtColumnPut (secondRowIndex, firstColumnIndex, cumulate => cumulate, 3);
			aMatrix.atRowAtColumnPut (secondRowIndex, secondColumnIndex, cumulate => cumulate, -7);
			aMatrix.atRowAtColumnPut (secondRowIndex, thirdColumnIndex, cumulate => cumulate, 6);
			aMatrix.atRowAtColumnPut (thirdRowIndex, firstColumnIndex, cumulate => cumulate, 4);
			aMatrix.atRowAtColumnPut (thirdRowIndex, secondColumnIndex, cumulate => cumulate, 1);
			aMatrix.atRowAtColumnPut (thirdRowIndex, thirdColumnIndex, cumulate => cumulate, 5);

			Vector<Object> aVector = new Vector<object> ();

			aVector.atPut (firstRowIndex, -7);
			aVector.atPut (secondRowIndex, 38);
			aVector.atPut (thirdRowIndex, 43);

			var result = aMatrix.Solve (aVector);

			var aList = new Dictionary<Object, int> ();
			aList.Add (firstRowIndex, 0);
			aList.Add (secondRowIndex, 1);
			aList.Add (thirdRowIndex, 2);

			var computedResult = result.forComputationAmong (aList, new ValueHolderCarryInfo<double>{Value = 0});

			var expectedResult = new DenseVector (new []{3.0,1.0,6.0});

			Assert.AreEqual (expectedResult, computedResult);
			
			
		}
	}
}

