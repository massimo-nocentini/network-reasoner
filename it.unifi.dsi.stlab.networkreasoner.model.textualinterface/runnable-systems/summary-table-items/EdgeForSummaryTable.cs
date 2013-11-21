using System;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	class EdgeForSummaryTable : SummaryTableItem
	{
		public Double Qvalue{ get; set; }

		// TODO: compute here the velocity

			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.SummaryTableItem
		public override void appendValueInto (StringBuilder table)
		{
			table.Append (formatDouble (Qvalue));
		}

		public override void appendHeaderInto (StringBuilder table)
		{
			table.AppendFormat ("Q_{0}", Identifier);
		}
			#endregion
	}

}

