using System;
using NUnit.Framework;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.console.emacs
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestCase ()
		{
			new MainClass ().run (new System.Collections.Generic.List<string> (
				File.ReadAllLines (
					"emacs-buffers-examples/network-for-pressure-regulator-inversions.org")
			)
			);
		}
	}
}

