using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class ReachabilityValidatorTests
	{
		[Test()]
		public void load_a_complete_network_with_a_main_object_defined_in_specification ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-a-reachability-validator.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network.ReachabilityValidator);
			Assert.IsTrue (network.ReachabilityValidator.Enabled);
			Assert.IsTrue (network.ReachabilityValidator.RaiseException);
			Assert.IsFalse (network.ReachabilityValidator.WriteLog);
		}

		[Test()]
		public void validate_a_connected_graph ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-a-reachability-validator.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network.ReachabilityValidator);
			Assert.IsTrue (network.ReachabilityValidator.Enabled);
			Assert.IsTrue (network.ReachabilityValidator.RaiseException);
			Assert.IsFalse (network.ReachabilityValidator.WriteLog);

			network.ReachabilityValidator.validate (network);
		}

		[Test()]
		[ExpectedException( typeof( NetworkNotConnectedException ) )]
		public void validate_a_not_connected_graph ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-a-reachability-validator-for-not-connected-network.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network.ReachabilityValidator);
			Assert.IsTrue (network.ReachabilityValidator.Enabled);
			Assert.IsTrue (network.ReachabilityValidator.RaiseException);
			Assert.IsFalse (network.ReachabilityValidator.WriteLog);

			network.ReachabilityValidator.validate (network);
		}
	}
}

