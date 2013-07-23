using System;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class Vector_minusMethodTest
	{
		[Test()]
		public void TestCase ()
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

			var aList = new List<Tuple<Object, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				aFirstIndex, 0, fromVector => fromVector
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				aSecondIndex, 1, fromVector => fromVector
			)
			);
			aList.Add (new Tuple<Object, int, Func<Double, Double>> (
				aThirdIndex, 2, fromVector => fromVector
			)
			);

			var leftMinusRightVector = aVectorLeft.minus (aVectorRight)
				.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new []{
				aFirstDoubleLeft - aFirstDoubleRight,
				aSecondDoubleLeft - aSecondDoubleRight,
				aThirdDoubleLeft - aThirdDoubleRight
			}
			);

			Assert.AreEqual (expectedVector, leftMinusRightVector);
			
		}
	}
}

