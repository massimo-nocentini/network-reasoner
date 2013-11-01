using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class ConditionCheckerEnabled : it.unifi.dsi.stlab.math.algebra.ConditionChecker
	{

		#region implemented abstract members of it.unifi.dsi.stlab.math.algebra.ConditionChecker
		public override void ensure (AlgebraConstraint aConstraint)
		{
			aConstraint.check ();
		}
		#endregion
	}

}

