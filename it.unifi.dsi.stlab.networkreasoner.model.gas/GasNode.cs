using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNode : GasNodeAbstract
	{


		public GasNode ()
		{
		}		

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeAbstract
		public override NodeMatrixConstruction adapterForMatrixConstruction ()
		{
			throw new System.NotImplementedException ();
		}
		#endregion


	}
}

