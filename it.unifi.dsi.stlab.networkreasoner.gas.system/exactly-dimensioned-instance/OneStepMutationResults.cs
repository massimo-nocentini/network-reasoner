using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public  class OneStepMutationResults
	{
		internal Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix {
			get;
			set;
		}

		internal Vector<NodeForNetwonRaphsonSystem> Unknowns {
			get;
			set;
		}

		internal Vector<NodeForNetwonRaphsonSystem> Coefficients {
			get;
			set;
		}

		internal Vector<EdgeForNetwonRaphsonSystem> Qvector {
			get;
			set;
		}

		internal Matrix<NodeForNetwonRaphsonSystem, 
					NodeForNetwonRaphsonSystem> Jacobian {
			get;
			set;
		}

		internal Vector<EdgeForNetwonRaphsonSystem> Fvector {
			get;
			set;
		}







	}
}

