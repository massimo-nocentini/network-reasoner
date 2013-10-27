using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class RunnableSystemAbstractComputationalResultHandlerShortTableSummary :
		RunnableSystemAbstractComputationalResultHandler
	{
		abstract class SummaryTableItem
		{
			public abstract void appendValueInto (StringBuilder table);

			public String Identifier{ get; set; }

			public int ColumnPosition{ get; set; }

			protected virtual String formatDouble (Double value)
			{
				return value.ToString ("E3");
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
			#endregion
		}

		abstract class TimeOfComputationHandling
		{
			public abstract void buildColumns (
				Dictionary<String, int> nodeColumnIndexesByNodeIdentifiers, 
				List<NodeForNetwonRaphsonSystem> nodesEnumerationUsedBySystemSolution,
				List<EdgeForNetwonRaphsonSystem> edgesEnumerationUsedBySystemSolution);

			public abstract TimeOfComputationHandling advance ();
		}

		class TimeOfComputationHandlingFirstOne : TimeOfComputationHandling
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.TimeOfComputationHandling
			public override void buildColumns (
				Dictionary<String, int> summaryLineItems, 
				List<NodeForNetwonRaphsonSystem> nodesEnumerationUsedBySystemSolution,
				List<EdgeForNetwonRaphsonSystem> edgesEnumerationUsedBySystemSolution)
			{
				// we start from position 1 since the first column is reserved for system name
				int columnPosition = 0;

				nodesEnumerationUsedBySystemSolution.ForEach (aNode => {

					summaryLineItems.Add (aNode.Identifier, columnPosition);

					columnPosition = columnPosition + 1;
				}
				);

				edgesEnumerationUsedBySystemSolution.ForEach (anEdge => {

					summaryLineItems.Add (anEdge.identifier (), columnPosition);

					columnPosition = columnPosition + 1;
				}
				);
			}

			public override TimeOfComputationHandling advance ()
			{
				return new TimeOfComputationHandlingBeyondFirst ();
			}
			#endregion
		}

		class TimeOfComputationHandlingBeyondFirst : TimeOfComputationHandling
		{
			#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.textualinterface.RunnableSystemAbstractComputationalResultHandlerShortTableSummary.TimeOfComputationHandling
			public override void buildColumns (
				Dictionary<String, int> nodeColumnIndexesByNodeIdentifiers, 
				List<NodeForNetwonRaphsonSystem> nodesEnumerationUsedBySystemSolution,
				List<EdgeForNetwonRaphsonSystem> edgesEnumerationUsedBySystemSolution)
			{
				// nothing to do here because the dictionary is filled by the first time the result-handler 
				// is called (this object represent from the second time and following)
			}

			public override TimeOfComputationHandling advance ()
			{
				return this;
			}
			#endregion
		}

		Dictionary<String, List<SummaryTableItem>> SummaryTableItems{ get; set; }

		TimeOfComputationHandling ComputationHandlingTime{ get; set; }

		Dictionary<String, int> NodesOrEdgesColumnIndexesByNodeOrEdgeObject { get; set; }

		public RunnableSystemAbstractComputationalResultHandlerShortTableSummary ()
		{
			SummaryTableItems = new Dictionary<string, List<SummaryTableItem>> ();
			ComputationHandlingTime = new TimeOfComputationHandlingFirstOne ();
			NodesOrEdgesColumnIndexesByNodeOrEdgeObject = new Dictionary<String, int> ();
		}

		protected override void onComputationFinished (
			string systemName, OneStepMutationResults results)
		{
			// here we build the ordering only for the first call of this method.
			ComputationHandlingTime.buildColumns (
				NodesOrEdgesColumnIndexesByNodeOrEdgeObject,
				results.ComputedBy.Nodes,
				results.ComputedBy.Edges);

			var dimensionalUnknowns = results.ComputedBy.
					makeUnknownsDimensional (results.Unknowns);

			List<SummaryTableItem> summaryTableItemsForCurrentSystem = 
				new List<SummaryTableItem> ();

			Dictionary<NodeForNetwonRaphsonSystem, double> sumOfQsByNodes = 
					new Dictionary<NodeForNetwonRaphsonSystem, double> ();

			// initialize the dictionary
			results.ComputedBy.Nodes.ForEach (
					aNode => sumOfQsByNodes.Add (aNode, 0));

			results.ComputedBy.Edges.ForEach (
					anEdge => {

				var Qvalue = results.Qvector.valueAt (anEdge);

				EdgeForSummaryTable summaryEdge = new EdgeForSummaryTable{
					Identifier = anEdge.identifier(),
					Qvalue = Qvalue,
					ColumnPosition = NodesOrEdgesColumnIndexesByNodeOrEdgeObject[anEdge.identifier()]
				};
				summaryTableItemsForCurrentSystem.Add (summaryEdge);

				sumOfQsByNodes [anEdge.StartNode] += Qvalue;
				sumOfQsByNodes [anEdge.EndNode] -= Qvalue;
			}
			);

			foreach (var pair in sumOfQsByNodes) {

				NodeForSummaryTable summaryNode = new NodeForSummaryTable{
					ColumnPosition = this.NodesOrEdgesColumnIndexesByNodeOrEdgeObject[pair.Key.Identifier],
					Identifier = pair.Key.Identifier,
					QvalueSum = pair.Value,
					DimensionalPressure = dimensionalUnknowns.valueAt(pair.Key)
				};

				summaryTableItemsForCurrentSystem.Add (summaryNode);
			}

			this.SummaryTableItems.Add (systemName, summaryTableItemsForCurrentSystem);

			ComputationHandlingTime = ComputationHandlingTime.advance ();

		}

		public virtual String buildTableSummary ()
		{
			StringBuilder table = new StringBuilder ();

			foreach (var pair in this.SummaryTableItems) {
				table.Append (pair.Key + "\t");

				for (int columnIndex = 0; 
				     columnIndex < pair.Value.Count; 
				     columnIndex = columnIndex + 1) {

					SummaryTableItem item = pair.Value.Find (
						anItem => anItem.ColumnPosition == columnIndex);

					item.appendValueInto (table);

					table.Append ("\t");
				}

				table.Append ("\n");
			}

			return table.ToString ();
		}
	}
}

