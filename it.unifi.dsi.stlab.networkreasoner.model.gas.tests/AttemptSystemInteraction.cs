using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.model.rdfinterface;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas.tests
{
	[TestFixture()]
	public class AttemptSystemInteraction
	{


		[Test()]
		public void check_if_KeyValuePair_is_value_object ()
		{
			GasNetwork g = new GasNetwork ();
			String s = "";

			var aPair = new KeyValuePair<GasNetwork, String> (g, s);
			var anotherPair = new KeyValuePair<GasNetwork, String> (g, s);

			var aDictionary = new Dictionary<KeyValuePair<GasNetwork, String>, String> ();

			aDictionary.Add (aPair, "");

			Assert.IsTrue (aDictionary.ContainsKey (anotherPair));
		}

		[Test()]
		public void check_if_KeyValuePair_is_value_object_unless_different_second_component ()
		{
			GasNetwork g = new GasNetwork ();

			var aPair = new KeyValuePair<GasNetwork, String> (g, "");
			var anotherPair = new KeyValuePair<GasNetwork, String> (g, "8");

			var aDictionary = new Dictionary<KeyValuePair<GasNetwork, String>, String> ();

			aDictionary.Add (aPair, "");

			Assert.IsFalse (aDictionary.ContainsKey (anotherPair));
		}

		[Test()]
		public void check_if_KeyValuePair_is_value_object_unless_different_first_component ()
		{
			String s = "";

			var aPair = new KeyValuePair<GasNetwork, String> (new GasNetwork (), s);
			var anotherPair = new KeyValuePair<GasNetwork, String> (new GasNetwork (), s);

			var aDictionary = new Dictionary<KeyValuePair<GasNetwork, String>, String> ();

			aDictionary.Add (aPair, "");

			Assert.IsFalse (aDictionary.ContainsKey (anotherPair));
		}
	
	}
}

