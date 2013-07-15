using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class DotRepresentationValidatorTests
	{
		[Test()]
		public void check_that_dot_representation_validator_is_correctly_setup ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-a-viewer-validator-for-not-connected-network.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			Assert.IsNotNull (network.DotRepresentationValidator);
			Assert.IsTrue (network.DotRepresentationValidator.Enabled);
			Assert.IsTrue (network.DotRepresentationValidator.RaiseException);
			Assert.IsFalse (network.DotRepresentationValidator.WriteLog);

		}

		[Test()]
		public void write_a_simple_representation_to_a_file ()
		{
			var loader = SpecificationLoader.MakeNTurtleSpecificationLoader ();

			var filenameToParse = "../../nturtle-specifications/specification-for-loading-a-viewer-validator-for-not-connected-network.nt";

			var network = loader.Load<GasNetwork> (filenameToParse);

			network.DotRepresentationValidator.validate (network);

			Assert.IsTrue (File.Exists ("simple-output-for-not-connected-network.svg"));

		}
	}
}

