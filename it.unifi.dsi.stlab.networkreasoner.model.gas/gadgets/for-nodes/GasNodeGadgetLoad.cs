using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeGadgetLoad : GasNodeGadget
	{
		public GasNodeGadgetLoad ()
		{
		}

		public double Load { get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeGadget
		public override NodeMatrixConstruction dispatchForNodeMatrixConstructionOn (
			GasNodeWithGadget gasNodeWithGadget)
		{
			return gasNodeWithGadget.makeNodeMatrixConstructionForLoadGadget (this);
		}
		#endregion

	}
}

