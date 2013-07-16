using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface GasEdgeVisitor
	{
		void forPhysicalEdge (GasEdgePhysical gasEdgePhysical);

		void forTopologicalEdge (GasEdgeTopological gasEdgeTopological);

		void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget);


	}
}

