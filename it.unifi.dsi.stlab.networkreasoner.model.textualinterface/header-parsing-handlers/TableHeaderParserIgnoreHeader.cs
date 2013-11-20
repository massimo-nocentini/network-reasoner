using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class TableHeaderParserIgnoreHeader : TableHeaderParser
	{
		#region TableHeaderParser implementation
		public System.Collections.Generic.List<string> parse (
			System.Collections.Generic.List<string> region)
		{
			var headerHorizontalDivisorLineIndex = region.FindIndex (line => line.StartsWith ("|-"));

			var result = region;
			if (headerHorizontalDivisorLineIndex > -1) {
				result = region.GetRange (headerHorizontalDivisorLineIndex + 1, 
				                         region.Count - headerHorizontalDivisorLineIndex - 1);
			}

			return result;

		}
		#endregion
	}
}

