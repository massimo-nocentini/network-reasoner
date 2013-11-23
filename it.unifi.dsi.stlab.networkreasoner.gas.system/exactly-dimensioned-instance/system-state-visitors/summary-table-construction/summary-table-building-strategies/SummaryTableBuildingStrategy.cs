using System;
using System.Text;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table
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

