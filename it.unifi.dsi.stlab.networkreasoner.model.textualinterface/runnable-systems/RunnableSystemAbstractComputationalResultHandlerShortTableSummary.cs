using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Text;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.utilities.times_of_computation;
using System.Linq;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class RunnableSystemAbstractComputationalResultHandlerShortTableSummary :
		RunnableSystemAbstractComputationalResultHandler
	{
		internal abstract class SummaryTableItem
		{
			public abstract void appendValueInto (StringBuilder table);

			public abstract void appendHeaderInto (StringBuilder table);

			public String Identifier{ get; set; }

			public int ColumnPosition{ get; set; }

			protected virtual String formatDouble (Double value)
			{
				return value.ToString ("0.000");
			}
		}

		class NodeForSummaryTable :SummaryTableItem
		{
			public Double QvalueSum{ get; set; }

			public Double DimensionalPressure{ get; set; }	

			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.SummaryTableItem
			public override void appendValueInto (StringBuilder table)
			{
				table.Append (formatDouble (DimensionalPressure) + "\t" + 
					formatDouble (QvalueSum)
				);
			}

			public override void appendHeaderInto (StringBuilder table)
			{
				table.AppendFormat ("P_{0}\tSUMQ_{0}", Identifier);
			}
			#endregion
		}

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


		Dictionary<String, Dictionary<int, SummaryTableItem>> SummaryTableItems{ get; set; }

		TimeOfComputationHandling ComputationHandlingTime{ get; set; }

		Dictionary<string, int> NodesOrEdgesColumnIndexesByNodeOrEdgeObject { get; set; }

		public RunnableSystemAbstractComputationalResultHandlerShortTableSummary ()
		{
			SummaryTableItems = new Dictionary<string, Dictionary<int, SummaryTableItem>> ();
			ComputationHandlingTime = new TimeOfComputationHandlingFirstOne ();
			NodesOrEdgesColumnIndexesByNodeOrEdgeObject = 
				new Dictionary<string, int> ();
		}

		protected virtual int columnPositionOf (String tableItemIdentifier)
		{
			return NodesOrEdgesColumnIndexesByNodeOrEdgeObject 
					[tableItemIdentifier];
		}

		void buildColumnPositionsDictionaryOnlyOnFirstTimeThisMethodIsCalled (
			OneStepMutationResults results)
		{
			var columnPositionsForTableSummaryItemsAction = 
				new ActionTimeComputationOnFirstTime ();

			columnPositionsForTableSummaryItemsAction.Action = () => {
				int columnPosition = 0;

				results.ComputedBy.Nodes.ForEach (aNode => {
					NodesOrEdgesColumnIndexesByNodeOrEdgeObject.Add (
						aNode.Identifier, columnPosition);
					columnPosition = columnPosition + 1;
				}
				);

				results.ComputedBy.Edges.ForEach (anEdge => {
					NodesOrEdgesColumnIndexesByNodeOrEdgeObject.Add (
						anEdge.identifier (), columnPosition);
					columnPosition = columnPosition + 1;
				}
				);
			};

			ComputationHandlingTime.perform (
				columnPositionsForTableSummaryItemsAction);
		}

		protected override void onComputationFinished (
			string systemName, OneStepMutationResults results)
		{
			buildColumnPositionsDictionaryOnlyOnFirstTimeThisMethodIsCalled (results);

			var dimensionalUnknowns = results.ComputedBy.
					makeUnknownsDimensional (results.Unknowns);

			var summaryTableItemsForCurrentSystem = 
				new Dictionary<int, SummaryTableItem> ();

			Dictionary<NodeForNetwonRaphsonSystem, double> sumOfQsByNodes = 
					new Dictionary<NodeForNetwonRaphsonSystem, double> ();

			// initialize the dictionary
			results.ComputedBy.Nodes.ForEach (
					aNode => sumOfQsByNodes.Add (aNode, 0));

			results.ComputedBy.Edges.ForEach (
					anEdge => {

				var Qvalue = results.Qvector.valueAt (anEdge);

				var columnPosition = this.columnPositionOf (anEdge.identifier ());

				EdgeForSummaryTable summaryEdge = new EdgeForSummaryTable{
					Identifier = anEdge.identifier(),
					Qvalue = Qvalue,
					ColumnPosition = columnPosition
				};
				summaryTableItemsForCurrentSystem.Add (columnPosition, summaryEdge);

				sumOfQsByNodes [anEdge.StartNode] += Qvalue;
				sumOfQsByNodes [anEdge.EndNode] -= Qvalue;
			}
			);

			foreach (var pair in sumOfQsByNodes) {

				int columnPosition = this.columnPositionOf (pair.Key.Identifier);

				NodeForSummaryTable summaryNode = new NodeForSummaryTable{
					ColumnPosition = columnPosition,
					Identifier = pair.Key.Identifier,
					QvalueSum = pair.Value,
					DimensionalPressure = dimensionalUnknowns.valueAt(pair.Key)
				};

				summaryTableItemsForCurrentSystem.Add (columnPosition, summaryNode);
			}

			this.SummaryTableItems.Add (systemName, summaryTableItemsForCurrentSystem);

			ComputationHandlingTime = ComputationHandlingTime.advance ();

		}

		internal virtual void appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
			StringBuilder table, 
			Dictionary<int, SummaryTableItem> resultLineForFirstSystem, 
			ListExtensionMethods.ListItemDecoratedWithTimeComputation<string> timedDecoredItem)
		{
			ActionTimeComputation actionForFirstSystemLine = 
				new ActionTimeComputationOnFirstTime ();

			actionForFirstSystemLine.Action = () => {
				table.Append ("SYSNAME\t");
				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (aColumnIndex => {
					var item = resultLineForFirstSystem [aColumnIndex];
					item.appendHeaderInto (table);
					table.Append ("\t");
				}
				);
				table.Append ("\n");
			};

			timedDecoredItem.ComputationTime.perform (actionForFirstSystemLine);
		}

		public virtual String buildTableSummary ()
		{
			StringBuilder table = new StringBuilder ();

			SummaryTableItems.Keys.ToList ().DecoreWithTimeComputation ().ForEach (
				timedDecoredItem => {

				var resultLineForFirstSystem = SummaryTableItems [timedDecoredItem.Item];

				appendHeadersIntoTableOnlyOnFirstTimeThisMethodIsCalled (
					table, resultLineForFirstSystem, timedDecoredItem);

				resultLineForFirstSystem.Count.rangeFromZero ().ForEach (
					aColumnPosition => {

					SummaryTableItem item = resultLineForFirstSystem [aColumnPosition];
					item.appendValueInto (table);
					table.Append ("\t");
				}
				);

				table.Append ("\n");
			}
			);

			return table.ToString ();
		}
	}
}

