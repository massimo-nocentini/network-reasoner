using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public  class OneStepMutationResults
	{
		internal Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem, double> Amatrix {
			get;
			set;
		}

		internal Vector<NodeForNetwonRaphsonSystem, double> Unknowns {
			get;
			set;
		}

		internal Vector<NodeForNetwonRaphsonSystem, double> Coefficients {
			get;
			set;
		}

		internal Vector<EdgeForNetwonRaphsonSystem, double> Qvector {
			get;
			set;
		}

		internal Matrix<NodeForNetwonRaphsonSystem, 
					NodeForNetwonRaphsonSystem, 
					double> Jacobian {
			get;
			set;
		}

		internal Vector<EdgeForNetwonRaphsonSystem, double> Fvector {
			get;
			set;
		}







	}
}

