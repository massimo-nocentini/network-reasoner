using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public abstract class ConditionChecker
	{
		public abstract void ensure (AlgebraConstraint aConstraint);
	}
}

