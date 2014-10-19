using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.extension_methods;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	class NodeAnomalyFinder : GasNodeVisitor, GasNodeGadgetVisitor
	{
		public Action<String> ReportAnomaly {
			get;
			set;
		}

		public double NodeAlgebraicSumOfFlows {
			get;
			set;
		}

		public double NodePressure {
			get;
			set;
		}

		#region GasNodeVisitor implementation

		public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
		{
			// nothing to check
		}

		public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
		{
			gasNodeWithGadget.Gadget.accept (this);
			gasNodeWithGadget.Equipped.accept (this);
		}


		public void forNodeAntecedentInPressureReduction (
			GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
		{
			// TODO: ask Fabio if the check on min/max flow written
			// for the edge regulator case should be moved here 
			// about Q, the algebraic sum of the antecedent (or, worse, of the conseguent...
			// in this case we should create another node variant).
			gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
		}

		#endregion

		#region GasNodeGadgetVisitor implementation

		public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
		{
			// nothing to check
		}

		public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
		{
			if (this.NodeAlgebraicSumOfFlows.belongToInterval (
				    aSupplyGadget.MinQ, aSupplyGadget.MaxQ) == false) {
				ReportAnomaly.Invoke (
					string.Format ("Sum of flow {0} not in allowed interval.", 
						this.NodeAlgebraicSumOfFlows));
			}
		}

		#endregion
	}
}

