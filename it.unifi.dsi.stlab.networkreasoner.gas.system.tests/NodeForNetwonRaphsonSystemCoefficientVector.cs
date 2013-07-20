using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NodeForNetwonRaphsonSystemCoefficientVector
	{
		[Test()]
		public void TestCase ()
		{
			var aSetupPressure = 387.291;

			var aSupplier = new GasNodeWithGadget ();
			aSupplier.Gadget = new GasNodeGadgetSupply{ SetupPressure = aSetupPressure };
			aSupplier.Equipped = new GasNodeTopological ();

			var nodeForSystem = new NodeForNetwonRaphsonSystem ();
			nodeForSystem.initializeWith (aSupplier);

			var aVector = new Vector<NodeForNetwonRaphsonSystem, Double> ();
			nodeForSystem.putYourCoefficientInto (aVector);

			var aList = new List<Tuple<NodeForNetwonRaphsonSystem, int, Func<Double, Double>>> ();
			aList.Add (new Tuple<NodeForNetwonRaphsonSystem, int, Func<double, double>> (nodeForSystem, 0, aDouble => aDouble));
			var a = aVector.forComputationAmong (aList, 0);

			var expectedVector = new DenseVector (new[] { aSetupPressure });

			Assert.AreEqual (expectedVector, a);
		}
	}
}

