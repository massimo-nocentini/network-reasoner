using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extension_methods
{
	public static class IntegerExtensionMethods
	{
		public static List<int> rangeFromZero (this int count)
		{
			var range = new List<int> ();

			for (int i = 0; i < count; i = i + 1) {
				range.Add (i);
			}

			return range;
		}

		public static List<int> rangeTill (
			this int start, int till)
		{
			var range = new List<int> ();

			for (int i = start; i < till; i = i + 1) {
				range.Add (i);
			}

			return range;
		}

		public static bool belongToInterval (
			this double witness, double? min, double? max)
		{
			var result = true;

			if (min.HasValue) {
				result = result && (witness >= min.Value);
			}

			if (max.HasValue) {
				result = result && (witness <= max.Value);
			}

			return result;
		}
	}
}

