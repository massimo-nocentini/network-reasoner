using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class VectorTest
	{
		[Test()]
		public void add_an_element_to_vector_should_produce_a_vector_of_dimension_one ()
		{
			NodeForNetwonRaphsonSystem aNode = new NodeForNetwonRaphsonSystem ();
			Vector<NodeForNetwonRaphsonSystem> aVector = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var aValue = 45.32;

			aVector.atPut (aNode, aValue);

			var expectedVector = new DenseVector (new []{aValue});

			var aList = new Dictionary<NodeForNetwonRaphsonSystem, int> ();
			aList.Add (aNode, 0);
			Assert.AreEqual (expectedVector, aVector.forComputationAmong (aList, 0));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void after_adding_a_key_to_a_vector_isnt_possible_to_update_the_key_value ()
		{
			NodeForNetwonRaphsonSystem aNode = new NodeForNetwonRaphsonSystem ();
			Vector<NodeForNetwonRaphsonSystem> aVector = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var aFirstValue = 45.32;
			var aSecondValue = 145.32;

			aVector.atPut (aNode, aFirstValue);
			aVector.atPut (aNode, aSecondValue);
		}

		[Test()]
		public void inserting_value_for_key_and_retriving_it_should_produce_the_equal_value ()
		{
			/*
				Observe that this test prove that the inserted value for double (which is
				a value type) isn't returned as is, the same instance isn't returned. 
				Only an equivalent boolean is returned.
			 */

			NodeForNetwonRaphsonSystem aNode = new NodeForNetwonRaphsonSystem ();
			Vector<NodeForNetwonRaphsonSystem> aVector = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var aValue = 45.32;

			// by induction, we assume the following message behaves correctly
			// since it is covered by tests above.
			aVector.atPut (aNode, aValue);

			var getFromVector = aVector.valueAt (aNode);

			Assert.AreEqual (getFromVector, aValue);
		}

		[Test()]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void inserting_value_for_key_and_retrieving_by_different_key_isnt_allowed ()
		{
			/*
				Observe that this test prove that the inserted value for double (which is
				a value type) isn't returned as is, the same instance isn't returned. 
				Only an equivalent boolean is returned.
			 */

			NodeForNetwonRaphsonSystem aNode = new NodeForNetwonRaphsonSystem ();
			Vector<NodeForNetwonRaphsonSystem> aVector = 
				new Vector<NodeForNetwonRaphsonSystem> ();

			var aValue = 45.32;

			// by induction, we assume the following message behaves correctly
			// since it is covered by tests above.
			aVector.atPut (aNode, aValue);

			NodeForNetwonRaphsonSystem aForeignNode = new NodeForNetwonRaphsonSystem ();
			
			var getFromVector = aVector.valueAt (aForeignNode);
		}
	}
}

