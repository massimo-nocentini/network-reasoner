using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasNodeGadget
	{
		public abstract NodeMatrixConstruction dispatchForNodeMatrixConstructionOn (
			GasNodeWithGadget gasNodeWithGadget);

		public abstract void accept (GasNodeGadgetVisitor visitor);
	}
}

