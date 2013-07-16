using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public  class OneStepMutationResults
	{
		public OneStepMutationResults ()
		{
		}

		public Dictionary<KeyValuePair<NodeMatrixConstruction, NodeMatrixConstruction>, double> Matrix {
			get;
			set;
		}

		public Dictionary<NodeMatrixConstruction, double> Unknowns {
			get;
			set;
		}

		public Dictionary<NodeMatrixConstruction, double> Coefficients {
			get;
			set;
		}

		public Dictionary<GasEdgeTopological, double> Qvector {
			get;
			set;
		}




	}
}

