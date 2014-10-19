using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeAntecedentInPressureRegulator : GasNodeAbstract
	{
		public GasNodeAbstract RegulatorNode{ get; set; }

		public GasNodeAbstract ToppedNode{ get; set; }

		#region implemented abstract members of GasNodeAbstract

		public override void accept (GasNodeVisitor visitor)
		{
			visitor.forNodeAntecedentInPressureReduction (this);
		}

		#endregion
	}
}

