using System;
using NUnit.Framework;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface.tests
{
	[TestFixture()]
	public class SpecifingSimpleGasNetworks
	{
		[Test()]
		public void read_a_network_with_just_only_one_node_in_gas_context ()
		{
			var loader = SpecificationLoader.make_nturtle_specification_loader ();

			loader.LoadFile ("../../nturtle-specifications/gas/network-with-a-node.nt");
		

			//Assert.AreEqual (7, g.Triples.Count);
		}
	}
}

