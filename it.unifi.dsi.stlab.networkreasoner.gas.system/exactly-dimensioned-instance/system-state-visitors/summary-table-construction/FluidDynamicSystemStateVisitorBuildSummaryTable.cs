using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.times_of_computation;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.state_visitors.summary_table
{
	public class FluidDynamicSystemStateVisitorBuildSummaryTable : 
		FluidDynamicSystemStateVisitorWithSystemName
	{

		#region FluidDynamicSystemStateVisitorWithSystemName implementation

		public string SystemName{ get; set; }

		public void forBareSystemState (FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			throw new System.NotImplementedException ();
		}

		public void forUnsolvedSystemState (FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			throw new System.NotImplementedException ();
		}

		public void forMathematicallySolvedState (FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{
			onComputationFinished (SystemName, fluidDynamicSystemStateMathematicallySolved.MutationResult);
		}

		public void forNegativeLoadsCorrectedState (FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			onComputationFinished (SystemName, fluidDynamicSystemStateNegativeLoadsCorrected.FluidDynamicSystemStateMathematicallySolved.MutationResult);
		}
		#endregion

		Dictionary<String, Dictionary<int, SummaryTableItem>> SummaryTableNodes{ get; set; }

		Dictionary<String, Dictionary<int, SummaryTableItem>> SummaryTableEdges{ get; set; }

		TimeOfComputationHandling ComputationHandlingTime{ get; set; }

		Dictionary<string, int> NodesPositionsByItemIdentifiers { get; set; }

		Dictionary<string, int> EdgesPositionsByItemIdentifiers { get; set; }

		public FluidDynamicSystemStateVisitorBuildSummaryTable ()
		{
			SummaryTableEdges = new Dictionary<string, Dictionary<int, SummaryTableItem>> ();
			SummaryTableNodes = new Dictionary<string, Dictionary<int, SummaryTableItem>> ();
			ComputationHandlingTime = new TimeOfComputationHandlingFirstOne ();
			NodesPositionsByItemIdentifiers = new Dictionary<string, int> ();
			EdgesPositionsByItemIdentifiers = new Dictionary<string, int> ();
		}

		protected virtual int positionOfItemIn (
			String tableItemIdentifier, 
			Dictionary<string, int> positionsByIdentifiers)
		{
			return positionsByIdentifiers [tableItemIdentifier];
		}

		protected virtual void buildColumnPositionsDictionaryOnlyOnFirstTimeThisMethodIsCalled (
			OneStepMutationResults results)
		{
			var columnPositionsForTableSummaryItemsAction = 
				new ActionTimeComputationOnFirstTime ();

			columnPositionsForTableSummaryItemsAction.Action = () => {
		
				assignPositionsToIdentifiers (
					results.StartingUnsolvedState.Nodes,
					NodesPositionsByItemIdentifiers);

				assignPositionsToIdentifiers (
					results.StartingUnsolvedState.Edges,
					EdgesPositionsByItemIdentifiers);
			};

			ComputationHandlingTime.perform (
				columnPositionsForTableSummaryItemsAction);
		}

		protected virtual void assignPositionsToIdentifiers<T> (
			List<T> abstractComputationItems, 
			Dictionary<string, int> positionsByIdentifiers)
			where T:AbstractItemForNetwonRaphsonSystem
		{
			int position = 0;
			abstractComputationItems.ForEach (aNode => {
				positionsByIdentifiers.Add (aNode.Identifier, position);
				position = position + 1;
			}
			);
		}

		protected virtual void onComputationFinished (
			string systemName, OneStepMutationResults results)
		{
			buildColumnPositionsDictionaryOnlyOnFirstTimeThisMethodIsCalled (results);

			var dimensionalUnknowns = results.makeUnknownsDimensional ().WrappedObject;

			var summaryTableNodesForCurrentSystem = 
				new Dictionary<int, SummaryTableItem> ();

			var summaryTableEdgesForCurrentSystem = 
				new Dictionary<int, SummaryTableItem> ();

			Dictionary<NodeForNetwonRaphsonSystem, double> sumOfQsByNodes = 
					new Dictionary<NodeForNetwonRaphsonSystem, double> ();

			results.StartingUnsolvedState.Nodes.ForEach (
					aNode => sumOfQsByNodes.Add (aNode, 0));

			results.StartingUnsolvedState.Edges.ForEach (
					anEdge => {

				double? Qvalue = null;
				if (results.Qvector.containsKey (anEdge)) {
					Qvalue = results.Qvector.valueAt (anEdge);
					
					sumOfQsByNodes [anEdge.StartNode] -= Qvalue.Value;
					sumOfQsByNodes [anEdge.EndNode] += Qvalue.Value;
				}

				double? VelocityValue = null; 
				if (results.VelocityVector.containsKey (anEdge)) {
					VelocityValue = results.VelocityVector.valueAt (anEdge);
				}

				var edgePosition = positionOfItemIn (anEdge.Identifier, 
				                                       EdgesPositionsByItemIdentifiers);

				EdgeForSummaryTable summaryEdge = new EdgeForSummaryTable{
					Identifier = anEdge.Identifier,
					IdentifierAsLinkNotation = anEdge.identifierUsingLinkNotation(),
					Qvalue = Qvalue,
					VelocityValue = VelocityValue,
					Position = edgePosition
				};

				summaryTableEdgesForCurrentSystem.Add (edgePosition, summaryEdge);

			}
			);

			foreach (var pair in sumOfQsByNodes) {

				int nodePosition = positionOfItemIn (pair.Key.Identifier,
				                                     NodesPositionsByItemIdentifiers);

				NodeForSummaryTable summaryNode = new NodeForSummaryTable{
					Position = nodePosition,
					Identifier = pair.Key.Identifier,
					QvalueSum = pair.Value,
					DimensionalPressure = dimensionalUnknowns.valueAt(pair.Key)
				};

				summaryTableNodesForCurrentSystem.Add (nodePosition, summaryNode);
			}

			SummaryTableEdges.Add (systemName, summaryTableEdgesForCurrentSystem);
			SummaryTableNodes.Add (systemName, summaryTableNodesForCurrentSystem);

			ComputationHandlingTime = ComputationHandlingTime.advance ();

		}

		public virtual String buildSummaryContent ()
		{
			StringBuilder table = new StringBuilder ();

			// we should check that they contains really the same items
			System.Diagnostics.Debug.Assert (SummaryTableEdges.Keys.Count == 
				SummaryTableNodes.Keys.Count
			);

			// here we've choosed SummaryTableEdges.Keys as argument to
			// build the strategy but its the same to use SummaryTableNodes.Keys
			SummaryTableBuildingStrategy strategyDependentOnRunAnalysis = 
				buildStrategy (SummaryTableEdges.Keys);

			strategyDependentOnRunAnalysis.collectNodesTableUsingInto (SummaryTableNodes, table);
	
			table.AppendFormat ("\n\n");

			strategyDependentOnRunAnalysis.collectEdgesTableUsingInto (SummaryTableEdges, table);

			return table.ToString ();
		}

		// here we consume a dictionary that is the same as the private field
		// since that field is private hence not accessible in a subclass.
		protected virtual SummaryTableBuildingStrategy buildStrategy (
			ICollection<string> summaryTableItems)
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

