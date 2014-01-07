using System;
using System.Collections.Generic;
using System.Linq;
using it.unifi.dsi.stlab.extension_methods;
using System.Text;
using it.unifi.dsi.stlab.utilities.times_of_computation;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table
{
	public class SummaryTableBuildingStrategyForMultiRunAnalysis :
		SummaryTableBuildingStrategy
	{
		const string ColumnSeparator = "|";
		const string RowSeparator = "|-";


		#region SummaryTableBuildingStrategy implementation
		public void collectNodesTableUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableNodes, 
			System.Text.StringBuilder table)
		{
			summaryTableNodes.Keys.ToList ().DecoreWithTimeComputation ().ForEach (
				timedDecoredItem => {

				var resultLineForFirstSystem = summaryTableNodes [timedDecoredItem.Item];

				appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
					table, resultLineForFirstSystem, timedDecoredItem);

				table.AppendFormat ("{1}{0}{1}", timedDecoredItem.Item, ColumnSeparator);

				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (
					aColumnPosition => {

					SummaryTableItem item = resultLineForFirstSystem [aColumnPosition];
					item.appendValueInto (table);
					table.Append (ColumnSeparator);
				}
				);

				table.Append ("\n");
			}
			);
			table.Append (RowSeparator);
		}

		public void collectEdgesTableUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableEdges, 
			StringBuilder table)
		{
			collectNodesTableUsingInto (summaryTableEdges, table);
		}

		#endregion

		protected virtual void appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
			StringBuilder table, 
			Dictionary<int, SummaryTableItem> resultLineForFirstSystem, 
			ListExtensionMethods.ListItemDecoratedWithTimeComputation<string> timedDecoredItem)
		{
			ActionTimeComputation actionForFirstSystemLine = 
				new ActionTimeComputationOnFirstTime ();

			actionForFirstSystemLine.Action = () => {
				table.AppendFormat ("{1}\n{0} SYSNAME {0}", ColumnSeparator, RowSeparator);
				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (
					aColumnPosition => {

					var item = resultLineForFirstSystem [aColumnPosition];
					item.appendHeaderInto (table);
					table.Append (ColumnSeparator);
				}
				);
				table.AppendFormat ("\n{0}\n", RowSeparator);
			};

			timedDecoredItem.ComputationTime.perform (actionForFirstSystemLine);
		}

	}
}

