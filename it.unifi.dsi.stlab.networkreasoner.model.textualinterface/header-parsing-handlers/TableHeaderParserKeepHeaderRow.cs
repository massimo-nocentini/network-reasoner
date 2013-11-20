using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class TableHeaderParserKeepHeaderRow : TableHeaderParser
	{
		#region TableHeaderParser implementation
		public System.Collections.Generic.List<string> parse (
			System.Collections.Generic.List<string> region)
		{
			region.RemoveAll (line => line.StartsWith ("|-"));
			return region;
		}
		#endregion
	}
}

