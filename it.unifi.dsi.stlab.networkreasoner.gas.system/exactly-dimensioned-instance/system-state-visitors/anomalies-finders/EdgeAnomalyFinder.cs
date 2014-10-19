using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.extension_methods;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	class EdgeAnomalyFinder : GasEdgeVisitor, GasEdgeGadgetVisitor
	{
		public Action<String> ReportAnomaly {
			get;
			set;
		}

		public double Velocity {
			get;
			set;
		}

		public double Flow {
			get;
			set;
		}

		#region GasEdgeVisitor implementation

		public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
		{
			if (this.Velocity.belongToInterval (
				    double.NegativeInfinity, gasEdgePhysical.MaxSpeed) == false) {
				ReportAnomaly.Invoke (
					string.Format (
						"Edge speed {0} greater than the max {1} allowed.", 
						this.Velocity,
						gasEdgePhysical.MaxSpeed));
			}
			gasEdgePhysical.Described.accept (this);
		}

		public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
		{
			// nothing to check
		}

		public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
		{
			gasEdgeWithGadget.Gadget.accept (this);
			gasEdgeWithGadget.Equipped.accept (this);
		}

		#endregion

		#region GasEdgeGadgetVisitor implementation

		public void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
		{
			// nothing to check
		}

		public void forPressureRegulatorGadget (GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator)
		{
			if (this.Flow.belongToInterval (
				    gasEdgeGadgetPressureRegulator.MinFlow, 
				    gasEdgeGadgetPressureRegulator.MaxFlow) == false) {
				ReportAnomaly.Invoke (
					string.Format (
						"Regulator edge's flow {0} doesn't belong to {1}...{2} requested interval.", 
						this.Flow,
						gasEdgeGadgetPressureRegulator.MinFlow, 
						gasEdgeGadgetPressureRegulator.MaxFlow));
			}
		}

		#endregion
	}
}

