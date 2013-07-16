using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface GasNodeGadgetVisitor
	{
		void ForLoadGadget (GasNodeGadgetLoad aLoadGadget);

		void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget);

	}
}

