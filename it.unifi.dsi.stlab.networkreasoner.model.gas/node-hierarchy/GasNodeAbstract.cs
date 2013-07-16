using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class GasNodeAbstract
	{
		public abstract void accept (GasNodeVisitor visitor);

		public abstract NodeMatrixConstruction adapterForMatrixConstruction ();
	}
}

