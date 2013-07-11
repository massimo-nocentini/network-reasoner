using System;

namespace it.unifi.dsi.stlab.networkreasoner.model
{
	public class MeasureUnitFactory
	{
		public MeasureUnitFactory ()
		{
		}

		public MeasureUnitBar.MeasureUnitBarBuilder bar ()
		{
			return MeasureUnitBar.newInstance();
		}

	}
}

