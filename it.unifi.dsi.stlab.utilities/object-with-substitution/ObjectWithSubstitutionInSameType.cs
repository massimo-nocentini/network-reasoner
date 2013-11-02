using System;

namespace it.unifi.dsi.stlab.utilities.object_with_substitution
{
	public class ObjectWithSubstitutionInSameType<T>
	{
		public T Original{ get; set; }

		public T Substituted{ get; set; }
	}
}

