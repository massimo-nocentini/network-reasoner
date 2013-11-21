using System;
using System.Collections.Generic;
using System.Linq;
using it.unifi.dsi.stlab.extensionmethods;
using System.Text;
using it.unifi.dsi.stlab.utilities.times_of_computation;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class SummaryTableBuildingStrategyForSingleRunAnalysis:
		SummaryTableBuildingStrategy
	{
		const string ColumnSeparator = "|";
		const string RowSeparator = "|-";


		#region SummaryTableBuildingStrategy implementation
		public void collectUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableItems, 
			System.Text.StringBuilder table)
		{
			summaryTableItems.Keys.ToList ().DecoreWithTimeComputation ().ForEach (
				timedDecoredItem => {

				Dictionary<int, SummaryTableItem> resultLineForFirstSystem = 
					summaryTableItems [timedDecoredItem.Item];

				appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
					table, timedDecoredItem);

				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (
					aRowPosition => {

					SummaryTableItem item = resultLineForFirstSystem [aRowPosition];
					table.AppendFormat ("{0} {1} {0}", ColumnSeparator, item.Identifier);
					item.appendValueInto (table);
					table.AppendFormat ("{0}\n", ColumnSeparator);
				}
				);
			}
			);
			table.Append (RowSeparator);
		}
		#endregion

		protected virtual void appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
			StringBuilder table, 
			ListExtensionMethods.ListItemDecoratedWithTimeComputation<string> timedDecoredItem)
		{
			ActionTimeComputation actionForFirstSystemLine = 
				new ActionTimeComputationOnFirstTime ();

			actionForFirstSystemLine.Action = () => table.AppendFormat (
				"{1}\n{0} NODE ID {0} PRESSURE {0} ALGEB FLOWS SUM {0}\n{1}\n", 
				ColumnSeparator, 
				RowSeparator);

			timedDecoredItem.ComputationTime.perform (actionForFirstSystemLine);
		}

	}
}

