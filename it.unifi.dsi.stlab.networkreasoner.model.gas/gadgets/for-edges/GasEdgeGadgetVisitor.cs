using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface GasEdgeGadgetVisitor
	{
		void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff);

		void forPressureRegulatorGadget (GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator);
	}
}

