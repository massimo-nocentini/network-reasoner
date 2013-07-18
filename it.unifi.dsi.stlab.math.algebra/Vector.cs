using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType, VType>
	{
		Dictionary<IndexType, VType> aVector;

		public VType valueAt (IndexType index)
		{
			throw new NotImplementedException ();
		}

		public void atPut (IndexType index, VType value)
		{
			throw new NotImplementedException ();
		}

	}
}

