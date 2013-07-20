using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public  class OneStepMutationResults
	{
		internal Matrix<NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, double> Amatrix {
			get;
			set;
		}

		internal Vector<NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, double> Unknowns {
			get;
			set;
		}

		internal Vector<NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, double> Coefficients {
			get;
			set;
		}

		internal Vector<NetwonRaphsonSystem.EdgeForNetwonRaphsonSystem, double> Qvector {
			get;
			set;
		}

		internal Matrix<NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, 
					NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, 
					double> Jacobian {
			get;
			set;
		}

		internal Vector<NetwonRaphsonSystem.EdgeForNetwonRaphsonSystem, double> Fvector {
			get;
			set;
		}







	}
}

