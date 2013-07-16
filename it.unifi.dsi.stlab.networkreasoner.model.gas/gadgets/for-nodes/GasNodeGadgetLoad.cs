using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeGadgetLoad : GasNodeGadget
	{
		public double Load { get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeGadget
		public override NodeMatrixConstruction dispatchForNodeMatrixConstructionOn (
			GasNodeWithGadget gasNodeWithGadget)
		{
			return gasNodeWithGadget.makeNodeMatrixConstructionForLoadGadget (this);
		}

		public override void accept (GasNodeGadgetVisitor visitor)
		{
			visitor.ForLoadGadget (this);
		}
		#endregion


	}
}

