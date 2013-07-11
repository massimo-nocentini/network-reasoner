using System;

namespace it.unifi.dsi.stlab.networkreasoner.model
{
	public class MeasureUnitBar
	{
		public class MeasureUnitBarBuilder
		{
			private MeasureUnitBar MeasureUnitBar{ get; set; }

			public MeasureUnitBarBuilder (MeasureUnitBar measureUnitBar)
			{
				this.MeasureUnitBar = measureUnitBar;
			}

			public MeasureUnitBarBuilder netwon (double newtons)
			{
				this.MeasureUnitBar.Netwon = newtons;
				return this;
			}

			public MeasureUnitBarBuilder square_centimeters (double cm2)
			{
				this.MeasureUnitBar.Squared_centimeters = cm2;
				return this;
			}

			public MeasureUnitBar make ()
			{
				return this.MeasureUnitBar;
			}
		}

		private Double Netwon{ get; set; }

		private Double Squared_centimeters{ get; set; }

		private MeasureUnitBar ()
		{
		}

		public static MeasureUnitBarBuilder newInstance ()
		{
			return new MeasureUnitBarBuilder (new MeasureUnitBar ());
		}
	}
}

