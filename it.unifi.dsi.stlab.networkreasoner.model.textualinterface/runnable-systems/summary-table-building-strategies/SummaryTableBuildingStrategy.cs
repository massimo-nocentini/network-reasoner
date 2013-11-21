using System;
using System.Text;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public interface SummaryTableBuildingStrategy
	{
		void collectUsingInto (Dictionary<string, Dictionary<int,  SummaryTableItem>> summaryTableItems, StringBuilder table);
	}
}

