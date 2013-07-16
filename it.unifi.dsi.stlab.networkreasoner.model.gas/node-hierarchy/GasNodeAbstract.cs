using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasNodeAbstract
	{
		public GasNodeAbstract ()
		{
		}

		public virtual String Identifier{ get; set; }

		public virtual String Comment{ get; set; }

		public virtual long Height{ get; set; }

		public abstract NodeMatrixConstruction adapterForMatrixConstruction ();
	}
}

