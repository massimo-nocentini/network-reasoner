using System;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table
{
	class EdgeForSummaryTable : SummaryTableItem
	{
		public Double? Qvalue{ get; set; }
		
		public Double? VelocityValue{ get; set; }

		public String IdentifierAsLinkNotation{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.SummaryTableItem
		public override void appendValueInto (StringBuilder table)
		{
			table.AppendFormat ("{1} {0} {2}", 
			                    columnSeparator (), 
			                    Qvalue.HasValue ? formatDouble (Qvalue.Value) : string.Empty, 
			                    VelocityValue.HasValue ? formatDouble (VelocityValue.Value) : string.Empty);
		}

		public override void appendHeaderInto (StringBuilder table)
		{
			table.AppendFormat ("Q_{0} ({1}) {2} V_{0}", 
			                    Identifier, 
			                    IdentifierAsLinkNotation,
			                    columnSeparator());
		}

		public override void appendIdentifierForSingleRunAnalysisInto (StringBuilder table)
		{
			string columnSeparator = "|";
			table.AppendFormat ("{1} {0} {2}", 
			                    columnSeparator, 
			                    Identifier, 
			                    IdentifierAsLinkNotation);
		}
		#endregion
	}

}

