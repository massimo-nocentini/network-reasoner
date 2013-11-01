using System;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class ConditionCheckerDisabled : ConditionChecker
	{

		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.ConditionChecker
		public override void ensure (AlgebraConstraint aConstraint)
		{
			// since this class represent a disable checker, we do nothing 
			// with the given constraint.
		}
		#endregion
	}
}

