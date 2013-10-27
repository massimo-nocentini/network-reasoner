using System;
using NUnit.Framework;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface.tests
{
	[TestFixture()]
	public class SplittedGheoNetInputParsing
	{
		[Test()]
		public void read_simple_specification_for_gheonet ()
		{
			TextualGheoNetInputParser parser = new TextualGheoNetInputParser (
				"examples/simple-splitted-specification.dat");

			SystemRunnerFromTextualGheoNetInput systemRunner = 
				parser.parse (new SpecificationAssemblerSplitted (
					"examples/nodes-loads-pressures-for-splitted-specification.dat"));

			//systemRunner.run (new CheckParsing ());			
		}
	}
}

