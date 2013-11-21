using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Text;
using it.unifi.dsi.stlab.extensionmethods;
using it.unifi.dsi.stlab.utilities.times_of_computation;
using System.Linq;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class RunnableSystemAbstractComputationalResultHandlerShortTableSummary :
		RunnableSystemAbstractComputationalResultHandler
	{
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

		protected virtual void buildColumnPositionsDictionaryOnlyOnFirstTimeThisMethodIsCalled (
			OneStepMutationResults results)
		{
			var columnPositionsForTableSummaryItemsAction = 
				new ActionTimeComputationOnFirstTime ();

			columnPositionsForTableSummaryItemsAction.Action = () => {
				int columnPosition = 0;

				results.StartingUnsolvedState.Nodes.ForEach (aNode => {
					NodesOrEdgesColumnIndexesByNodeOrEdgeObject.Add (
						aNode.Identifier, columnPosition);
					columnPosition = columnPosition + 1;
				}
				);

				results.StartingUnsolvedState.Edges.ForEach (anEdge => {
					NodesOrEdgesColumnIndexesByNodeOrEdgeObject.Add (
						anEdge.Identifier, columnPosition);
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

			var dimensionalUnknowns = results.makeUnknownsDimensional ();

			var summaryTableItemsForCurrentSystem = 
				new Dictionary<int, SummaryTableItem> ();

			Dictionary<NodeForNetwonRaphsonSystem, double> sumOfQsByNodes = 
					new Dictionary<NodeForNetwonRaphsonSystem, double> ();

			// initialize the dictionary
			results.StartingUnsolvedState.Nodes.ForEach (
					aNode => sumOfQsByNodes.Add (aNode, 0));

			results.StartingUnsolvedState.Edges.ForEach (
					anEdge => {

				var Qvalue = results.Qvector.valueAt (anEdge);

				var columnPosition = this.columnPositionOf (anEdge.Identifier);

				EdgeForSummaryTable summaryEdge = new EdgeForSummaryTable{
					Identifier = anEdge.Identifier,
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
					DimensionalPressure = dimensionalUnknowns.WrappedObject.valueAt(pair.Key)
				};

				summaryTableItemsForCurrentSystem.Add (columnPosition, summaryNode);
			}

			this.SummaryTableItems.Add (systemName, summaryTableItemsForCurrentSystem);

			ComputationHandlingTime = ComputationHandlingTime.advance ();

		}

		public virtual String buildTableSummary ()
		{
			StringBuilder table = new StringBuilder ();

			SummaryTableBuildingStrategy strategyDependentOnRunAnalysis = 
				buildStrategy (SummaryTableItems);

			strategyDependentOnRunAnalysis.collectUsingInto (SummaryTableItems, table);

			return table.ToString ();
		}

		// here we consume a dictionary that is the same as the private field
		// since that field is private hence not accessible in a subclass.
		protected virtual SummaryTableBuildingStrategy buildStrategy (
			Dictionary<string, Dictionary<int, SummaryTableItem>> summaryTableItems)
		{
			SummaryTableBuildingStrategy strategy = null;

			if (summaryTableItems.Count == 1) { 
				strategy = new SummaryTableBuildingStrategyForSingleRunAnalysis ();
			} else {
				strategy = new SummaryTableBuildingStrategyForMultiRunAnalysis ();
			}

			return strategy;
		}

	}
}

