using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public  class OneStepMutationResults
	{
		public Matrix<NodeForNetwonRaphsonSystem, NodeForNetwonRaphsonSystem> Amatrix {
			get;
			set;
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> Unknowns {
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

		public int IterationNumber {
			get;
			set;
		}

		public FluidDynamicSystemStateUnsolved StartingUnsolvedState {
			get;
			set;
		}

		public formulae.GasFormulaVisitor UsedFormulae { 
			get; 
			set;
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> makeUnknownsDimensional ()
		{
			var translatorMaker = new dimensional_objects.DimensionalDelegates ();

			var translator = translatorMaker.makeAdimensionalToDimensionalTranslator (
				StartingUnsolvedState.Nodes, UsedFormulae);

			return Unknowns.makeDimensional (translator);
		}
	}
}

