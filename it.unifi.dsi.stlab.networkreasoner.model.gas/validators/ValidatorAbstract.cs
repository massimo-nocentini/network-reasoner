using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public abstract class ValidatorAbstract
	{
		public Boolean Enabled{ get; set; }

		public Boolean RaiseException{ get; set; }

		public Boolean WriteLog{ get; set; }

		public ValidatorAbstract ()
		{
		}
	}
}

