using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class NetwonRaphsonSystemLogging
	{
		[Test()]
		public void write_a_simple_message_should_produce_a_file ()
		{
			NetwonRaphsonSystem system = new NetwonRaphsonSystem();
			//system.writeSomeLog("Hello, first stupid info log message.");
		}
	}
}

