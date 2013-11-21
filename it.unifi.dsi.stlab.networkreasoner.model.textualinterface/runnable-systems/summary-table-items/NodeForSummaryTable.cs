using System;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	class NodeForSummaryTable :SummaryTableItem
	{
		public Double QvalueSum{ get; set; }

		public Double DimensionalPressure{ get; set; }	

			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.SummaryTableItem
		public override void appendValueInto (StringBuilder table)
		{
			table.AppendFormat ("{0} {2} {1}",
				                    formatDouble (DimensionalPressure),
				                    formatDouble (QvalueSum),
				                    columnSeparator ());
		}

		public override void appendHeaderInto (StringBuilder table)
		{
			table.AppendFormat ("P_{0} {1} SUMQ_{0}", Identifier, columnSeparator ());
		}

			#endregion
	}
}

