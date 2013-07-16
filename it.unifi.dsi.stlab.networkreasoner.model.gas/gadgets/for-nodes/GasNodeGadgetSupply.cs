using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeGadgetSupply : GasNodeGadget
	{

		public double SetupPressure { get; set; }

		public double MaxQ { get; set; }

		public double MinQ { get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeGadget
		public override NodeMatrixConstruction dispatchForNodeMatrixConstructionOn (
			GasNodeWithGadget gasNodeWithGadget)
		{
			return gasNodeWithGadget.makeNodeMatrixConstructionForSupplyGadget (this);
		}

		public override void accept (GasNodeGadgetVisitor visitor)
		{
			visitor.forSupplyGadget (this);
		}
		#endregion


	}
}

