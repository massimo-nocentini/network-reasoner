using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class RepeatUntilConditionRatioPrecisionReached
	{
		[Test()]
		public void two_vectors_with_identical_entries_should_stop_mutation ()
		{
			var previousUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();
			var currentUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

			NodeForNetwonRaphsonSystem n1 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n2 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n3 = new NodeForNetwonRaphsonSystem ();

			Double v1 = 3.14;
			Double v2 = 13.14;
			Double v3 = 23.14;

			previousUnknowns.atPut (n1, v1);
			previousUnknowns.atPut (n2, v2);
			previousUnknowns.atPut (n3, v3);

			currentUnknowns.atPut (n1, v1);
			currentUnknowns.atPut (n2, v2);
			currentUnknowns.atPut (n3, v3);

			var untilCondition = new UntilConditionAdimensionalRatioPrecisionReached{
				Precision = 1e-10
			};

			var previousUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = previousUnknowns
			};

			
			var currentUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = currentUnknowns
			};

			Assert.IsFalse (untilCondition.canContinue (
				new OneStepMutationResults{ Unknowns= previousUnknownsWrapper}, 
				new OneStepMutationResults{ Unknowns= currentUnknownsWrapper})
			);


		}

		[Test()]
		public void two_vectors_with_close_entries_should_stop_mutation ()
		{
			var previousUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();
			var currentUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

			NodeForNetwonRaphsonSystem n1 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n2 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n3 = new NodeForNetwonRaphsonSystem ();

			Double precision = 1e-10;
			Double epsilon = 1e-11;

			Double v1 = 3.14;
			Double v2 = 13.14;
			Double v3 = 23.14;

			previousUnknowns.atPut (n1, v1);
			previousUnknowns.atPut (n2, v2);
			previousUnknowns.atPut (n3, v3);

			currentUnknowns.atPut (n1, v1 + precision - epsilon);
			currentUnknowns.atPut (n2, v2 - precision + epsilon);
			currentUnknowns.atPut (n3, v3);

			var untilCondition = new UntilConditionAdimensionalRatioPrecisionReached{
				Precision = precision
			};

			var previousUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = previousUnknowns
			};

			
			var currentUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = currentUnknowns
			};


			Assert.IsFalse (untilCondition.canContinue (
				new OneStepMutationResults{ Unknowns= previousUnknownsWrapper}, 
				new OneStepMutationResults{ Unknowns= currentUnknownsWrapper})
			);


		}

		[Test()]
		public void two_vectors_with_entries_differing_on_edge_should_stop_mutation ()
		{
			var previousUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();
			var currentUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

			NodeForNetwonRaphsonSystem n1 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n2 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n3 = new NodeForNetwonRaphsonSystem ();

			Double precision = 1e-10;

			Double v1 = 3.14;
			Double v2 = 13.14;
			Double v3 = 23.14;

			previousUnknowns.atPut (n1, v1);
			previousUnknowns.atPut (n2, v2);
			previousUnknowns.atPut (n3, v3);

			currentUnknowns.atPut (n1, v1 + precision);
			currentUnknowns.atPut (n2, v2 - precision);
			currentUnknowns.atPut (n3, v3);

			var untilCondition = new UntilConditionAdimensionalRatioPrecisionReached{
				Precision = precision
			};

			var previousUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = previousUnknowns
			};

			
			var currentUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = currentUnknowns
			};

			Assert.IsFalse (untilCondition.canContinue (
				new OneStepMutationResults{ Unknowns= previousUnknownsWrapper}, 
				new OneStepMutationResults{ Unknowns= currentUnknownsWrapper})
			);


		}

		[Test()]
		public void second_vector_is_far_from_the_first_should_stop_mutation ()
		{
			var previousUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();
			var currentUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

			NodeForNetwonRaphsonSystem n1 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n2 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n3 = new NodeForNetwonRaphsonSystem ();

			Double precision = 1e-10;

			Double v1 = 3.14;
			Double v2 = 13.14;
			Double v3 = 23.14;

			previousUnknowns.atPut (n1, v1);
			previousUnknowns.atPut (n2, v2);
			previousUnknowns.atPut (n3, v3);

			currentUnknowns.atPut (n1, v1 + 4);
			currentUnknowns.atPut (n2, v2);
			currentUnknowns.atPut (n3, v3);

			var untilCondition = new UntilConditionAdimensionalRatioPrecisionReached{
				Precision = precision
			};

			var previousUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = previousUnknowns
			};

			
			var currentUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = currentUnknowns
			};

			Assert.IsTrue (untilCondition.canContinue (
				new OneStepMutationResults{ Unknowns= previousUnknownsWrapper}, 
				new OneStepMutationResults{ Unknowns= currentUnknownsWrapper})
			);


		}

		[Test()]
		public void first_vector_is_far_from_the_second_should_stop_mutation ()
		{
			var previousUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();
			var currentUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

			NodeForNetwonRaphsonSystem n1 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n2 = new NodeForNetwonRaphsonSystem ();
			NodeForNetwonRaphsonSystem n3 = new NodeForNetwonRaphsonSystem ();

			Double precision = 1e-10;

			Double v1 = 3.14;
			Double v2 = 13.14;
			Double v3 = 23.14;

			previousUnknowns.atPut (n1, v1 + 4);
			previousUnknowns.atPut (n2, v2);
			previousUnknowns.atPut (n3, v3);

			currentUnknowns.atPut (n1, v1);
			currentUnknowns.atPut (n2, v2);
			currentUnknowns.atPut (n3, v3);

			var untilCondition = new UntilConditionAdimensionalRatioPrecisionReached{
				Precision = precision
			};

			var previousUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = previousUnknowns
			};

			
			var currentUnknownsWrapper = new DimensionalObjectWrapperWithoutDimension<
			Vector<NodeForNetwonRaphsonSystem>> {
				WrappedObject = currentUnknowns
			};


			Assert.IsTrue (untilCondition.canContinue (
				new OneStepMutationResults{ Unknowns= previousUnknownsWrapper}, 
				new OneStepMutationResults{ Unknowns= currentUnknownsWrapper})
			);


		}

	}
}

