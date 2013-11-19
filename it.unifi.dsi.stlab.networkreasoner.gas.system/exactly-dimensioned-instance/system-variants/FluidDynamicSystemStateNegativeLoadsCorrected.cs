using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateNegativeLoadsCorrected 
		: FluidDynamicSystemStateAbstract
	{
		public FluidDynamicSystemStateTransitionNegativeLoadsChecker CorrectedBy {
			get;
			set;
		}

		public FluidDynamicSystemStateMathematicallySolved FluidDynamicSystemStateMathematicallySolved {
			get;
			set;
		}

		public List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> EdgesSubstitutions {
			get;
			set;
		}

		public List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> NodesSubstitutions {
			get;
			set;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.FluidDynamicSystemStateAbstract
		public override FluidDynamicSystemStateAbstract doStateTransition (FluidDynamicSystemStateTransition aVisitor)
		{
			return aVisitor.forNegativeLoadsCorrectedState (this);
		}

		public override void accept (FluidDynamicSystemStateVisitor aVisitor)
		{
			aVisitor.forNegativeLoadsCorrectedState (this);
		}
		#endregion
	}
}

