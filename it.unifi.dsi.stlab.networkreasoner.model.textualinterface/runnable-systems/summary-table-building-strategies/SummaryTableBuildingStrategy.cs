using System;
using System.Text;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public interface SummaryTableBuildingStrategy
	{
		void collectNodesTableUsingInto (
			Dictionary<string, Dictionary<int,  SummaryTableItem>> summaryTableNodes, 
			StringBuilder table);

		void collectEdgesTableUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableEdges, 
			StringBuilder table);

	}
}

