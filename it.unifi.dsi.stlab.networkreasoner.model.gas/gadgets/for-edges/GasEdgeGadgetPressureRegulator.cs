﻿using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	// TODO: use UPSTREAM instead of ANTECEDENT and the
	// corresponding version for consequent nomenclature.
	public class GasEdgeGadgetPressureRegulator : GasEdgeGadget
	{
		public Double? MinFlow{ get; set; }

		public Double? MaxFlow{ get; set; }

		#region implemented abstract members of GasEdgeGadget

		public override void accept (GasEdgeGadgetVisitor aVisitor)
		{
			aVisitor.forPressureRegulatorGadget (this);
		}

		#endregion
	}
}

