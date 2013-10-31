using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class ListExtensionMethods
	{
		public static List<T> Rest<T> (this List<T> aList)
		{
			return aList.IsEmpty () ? aList : aList.GetRange (1, aList.Count - 1);
		}

		public static bool IsEmpty<T> (this List<T> aList)
		{
			return aList.Count == 0;
		}

		public static T First<T> (this List<T> aList)
		{
			return aList [0];
		}
		
	}
}

