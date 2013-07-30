using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public struct AmatrixQuadruplet
	{
		public Func<double, double> StartNodeStartNodeUpdater{ get; set; }

		public double StartNodeStartNodeInitialValue {
			get;
			set;
		}

		public Func<double, double> StartNodeEndNodeUpdater{ get; set; }

		public double StartNodeEndNodeInitialValue {
			get;
			set;
		}

		public Func<double, double> EndNodeStartNodeUpdater{ get; set; }

		public double EndNodeStartNodeInitialValue {
			get;
			set;
		}

		public Func<double, double> EndNodeEndNodeUpdater{ get; set; }

		public double EndNodeEndNodeInitialValue {
			get;
			set;
		}
	}
}

