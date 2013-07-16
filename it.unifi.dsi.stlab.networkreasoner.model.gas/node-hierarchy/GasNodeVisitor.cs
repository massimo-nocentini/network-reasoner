using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface GasNodeVisitor
	{
		void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological);

		void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget);

	}
}

