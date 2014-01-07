using System;
using System.Collections.Generic;
using System.Linq;
using it.unifi.dsi.stlab.extension_methods;
using System.Text;
using it.unifi.dsi.stlab.utilities.times_of_computation;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table
{
	public class SummaryTableBuildingStrategyForSingleRunAnalysis:
		SummaryTableBuildingStrategy
	{
		const string ColumnSeparator = "|";
		const string RowSeparator = "|-";


		#region SummaryTableBuildingStrategy implementation
		public void collectNodesTableUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableNodes, 
			System.Text.StringBuilder table)
		{
			Action headerAction = () => table.AppendFormat (
				"{1}\n{0} NODE ID {0} PRESSURE {0} ALGEB FLOWS SUM {0}\n{1}\n", 
				ColumnSeparator, 
				RowSeparator);

			collectTableItemsInto (headerAction,
			                      summaryTableNodes,
			                      table);
		}

		public void collectEdgesTableUsingInto (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableEdges, 
			StringBuilder table)
		{
			Action headerAction = () => table.AppendFormat (
				"{1}\n{0} EDGE ID {0} LINK {0} FLOW {0} VELOCITY {0}\n{1}\n", 
				ColumnSeparator, 
				RowSeparator);

			collectTableItemsInto (headerAction,
			                      summaryTableEdges,
			                      table);
		}

		#endregion

		protected virtual void collectTableItemsInto (
			Action headerAction,
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableNodes, 
			System.Text.StringBuilder table)
		{
			summaryTableNodes.Keys.ToList ().DecoreWithTimeComputation ().ForEach (
				timedDecoredItem => {

				Dictionary<int, SummaryTableItem> resultLineForFirstSystem = 
					summaryTableNodes [timedDecoredItem.Item];

				appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
					headerAction, table, timedDecoredItem);

				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (
					aRowPosition => {

					SummaryTableItem item = resultLineForFirstSystem [aRowPosition];
					table.AppendFormat ("{0} ", ColumnSeparator);
					item.appendIdentifierForSingleRunAnalysisInto (table);
					table.AppendFormat (" {0}", ColumnSeparator);
					item.appendValueInto (table);
					table.AppendFormat ("{0}\n", ColumnSeparator);
				}
				);
			}
			);
			table.Append (RowSeparator);
		}

		protected virtual void appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
			Action action,
			StringBuilder table, 
			ListExtensionMethods.ListItemDecoratedWithTimeComputation<string> timedDecoredItem)
		{
			ActionTimeComputation actionForFirstSystemLine = 
			new ActionTimeComputationOnFirstTime {Action = action};

			timedDecoredItem.ComputationTime.perform (actionForFirstSystemLine);
		}		

		



	}
}

