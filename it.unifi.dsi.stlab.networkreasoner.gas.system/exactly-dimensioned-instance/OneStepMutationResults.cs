using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public  class OneStepMutationResults
	{
		public Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix {
			get;
			set;
		}

		public Vector<NodeForNetwonRaphsonSystem> Unknowns {
			get;
			set;
		}

		public Vector<NodeForNetwonRaphsonSystem> Coefficients {
			get;
			set;
		}

		public Vector<EdgeForNetwonRaphsonSystem> Qvector {
			get;
			set;
		}

		public Matrix<NodeForNetwonRaphsonSystem, 
					NodeForNetwonRaphsonSystem> Jacobian {
			get;
			set;
		}

		public Vector<EdgeForNetwonRaphsonSystem> Fvector {
			get;
			set;
		}







	}
}

