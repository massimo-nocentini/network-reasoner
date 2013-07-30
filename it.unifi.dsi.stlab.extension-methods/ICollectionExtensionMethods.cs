using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class ICollectionExtensionMethods
	{
		public static Dictionary<T, int> enumerate<T> (
			this ICollection<T> aSet)
		{
			Dictionary<T, int> anEnumeration = 
				new Dictionary<T, int> ();

			int anIndex = 0; 
			foreach (var anElement in aSet) {
				anEnumeration.Add (anElement, anIndex);
				anIndex = anIndex + 1;
			}

			return anEnumeration;
		}
	}
}

