using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extension_methods
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

		public static TValue MapOrDefault<TKey, TValue> (
			this Dictionary<TKey, TValue> aDictionary, TKey key, TValue defaultValue)
		{
			TValue delegatedValue = default(TValue);

			if (aDictionary.TryGetValue (key, out delegatedValue) == false) {
				delegatedValue = defaultValue;
			}

			return delegatedValue;
		}

		public static TKey MapIfPossible<TKey> (
			this Dictionary<TKey, TKey> aDictionary, TKey key)
		{
			return aDictionary.MapOrDefault (key, key);
		}
	}
}

