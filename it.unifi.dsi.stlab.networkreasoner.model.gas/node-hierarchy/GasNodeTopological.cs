using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;
using it.unifi.dsi.stlab.exceptions;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeTopological : GasNodeAbstract
	{
		public virtual String Identifier{ get; set; }

		public virtual String Comment{ get; set; }

		public virtual long Height{ get; set; }

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeAbstract
		public override NodeMatrixConstruction adapterForMatrixConstruction ()
		{
			throw new ShouldNotImplementException ();
		}
		#endregion


	}
}

