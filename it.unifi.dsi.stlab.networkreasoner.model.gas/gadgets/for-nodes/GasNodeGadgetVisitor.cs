using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface GasNodeGadgetVisitor
	{
		void forLoadGadget (GasNodeGadgetLoad aLoadGadget);

		void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget);

	}
}

