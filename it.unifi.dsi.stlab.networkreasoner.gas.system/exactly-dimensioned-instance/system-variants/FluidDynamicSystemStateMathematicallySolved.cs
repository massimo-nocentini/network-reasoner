using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateMathematicallySolved : FluidDynamicSystemStateAbstract
	{
		public OneStepMutationResults MutationResult {
			get;
			set;
		}

		public FluidDynamicSystemStateTransitionNewtonRaphsonSolve SolvedBy{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.FluidDynamicSystemStateAbstract
		public override FluidDynamicSystemStateAbstract doStateTransition (FluidDynamicSystemStateTransition aVisitor)
		{
			return aVisitor.forMathematicallySolvedState (this);
		}

		public override void accept (FluidDynamicSystemStateVisitor aVisitor)
		{
			aVisitor.forMathematicallySolvedState (this);
		}
		#endregion


	}
}

