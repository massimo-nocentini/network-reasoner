using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public  class OneStepMutationResults
	{
		internal Matrix<NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, NetwonRaphsonSystem.NodeForNetwonRaphsonSystem, double> Matrix {
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




	}
}

