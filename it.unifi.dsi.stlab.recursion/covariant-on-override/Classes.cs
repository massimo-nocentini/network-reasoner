using System;

namespace it.unifi.dsi.stlab.recursion
{
	public class BaseClass
	{
		public virtual BaseClass identity ()
		{
			return this;
		}
	}

	public class DerivedClass : BaseClass
	{
		public override BaseClass identity ()
		{
			return this;
		}
	}
}

