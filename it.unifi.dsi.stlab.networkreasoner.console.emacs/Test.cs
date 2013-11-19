using System;
using NUnit.Framework;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.console.emacs
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestCase ()
		{
			MainClass.Main (new []{
				File.ReadAllText ("emacs-buffers-examples/multirun-with-computation-parameters.org")}
			);
		}
	}
}

