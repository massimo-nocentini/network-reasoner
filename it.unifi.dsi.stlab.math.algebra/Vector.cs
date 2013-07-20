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

		public Vector<IndexType, VType> minus (
			Vector<IndexType, VType> aVector)
		{
			throw new NotImplementedException ();
		}

		public void doOnEach (Func<IndexType, VType, VType> updater)
		{
			var keys = aVector.Keys;
			foreach (var key in keys) {
				aVector [key] = updater.Invoke (key, aVector [key]);
			}
		}
	}
}

