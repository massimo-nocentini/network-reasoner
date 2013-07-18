using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra
{
	public class Vector<IndexType, VType>
	{
		Dictionary<IndexType, VType> aVector{ get; set; }

		public Vector ()
		{
			this.aVector = new Dictionary<IndexType, VType> ();
		}

		public VType valueAt (IndexType index)
		{
			return this.aVector [index];
		}

		public void atPut (IndexType index, VType value)
		{
			this.aVector.Add (index, value);
		}

	}
}

