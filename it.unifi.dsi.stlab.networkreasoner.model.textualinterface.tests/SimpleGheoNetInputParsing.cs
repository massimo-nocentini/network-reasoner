using System;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface.tests
{
	[TestFixture()]
	public class SimpleGheoNetInputParsing
	{
		[Test()]
		public void read_simple_specification_for_gheonet ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				"examples/first-simple-specification.dat");

			Dictionary<String, GasNodeAbstract> nodes = parser.parseNodes ();

			Dictionary<String, GasEdgeAbstract> edges = parser.parseEdgesRelating (nodes);

			AmbientParameters parameters = parser.parseAmbientParameters ();

			// implement here the assertions

			
		}
	}
}

