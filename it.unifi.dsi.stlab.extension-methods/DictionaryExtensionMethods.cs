using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class DictionaryExtensionMethods
	{
		public static Dictionary<TValue, TKey> invertMapping<TKey, TValue> (
			this Dictionary<TKey, TValue> aDictionary)
		{
			Dictionary<TValue, TKey> invertedDictionary = 
				new Dictionary<TValue, TKey> ();

			foreach (var pair in aDictionary) {
				invertedDictionary.Add (pair.Value, pair.Key);
			}

			return invertedDictionary;
		}

		public static void ForEach<TKey, TValue> (
			this Dictionary<TKey, TValue> aDictionary, Action<TKey, TValue> anAction)
		{
			foreach (var pair in aDictionary) {
				anAction.Invoke (pair.Key, pair.Value);
			}
		}
	}
}

