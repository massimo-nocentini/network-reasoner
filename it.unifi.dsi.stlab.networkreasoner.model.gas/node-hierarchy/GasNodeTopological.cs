using System;

using it.unifi.dsi.stlab.exceptions;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeTopological : GasNodeAbstract
	{
		public virtual String Identifier{ get; set; }

		public virtual String Comment{ get; set; }

		public virtual Nullable<long> Height{ get; set; }

		public override void accept (GasNodeVisitor visitor)
		{
			visitor.forNodeWithTopologicalInfo (this);
		}
	}
}

