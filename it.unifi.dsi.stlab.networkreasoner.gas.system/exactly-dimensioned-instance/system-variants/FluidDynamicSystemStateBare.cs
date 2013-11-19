using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	/// <summary>
	/// Fluid dynamic system state bare.
	/// 
	/// This class represent the concept of a bare system, that is a system that can be
	/// used as a starting point for a Newton-Raphson solution procedure. There's no
	/// reason to attach to it any information since up to now the system doesn't know
	/// the network which is under study, hence we use it as the primitive building block.
	/// </summary>
	public class FluidDynamicSystemStateBare : FluidDynamicSystemStateAbstract
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.gas.system.FluidDynamicSystemStateAbstract
		public override FluidDynamicSystemStateAbstract doStateTransition (
			FluidDynamicSystemStateTransition aVisitor)
		{
			return aVisitor.forBareSystemState (this);
		}

		public override void accept (FluidDynamicSystemStateVisitor aVisitor)
		{
			aVisitor.forBareSystemState(this);
		}
		#endregion
	}
}

