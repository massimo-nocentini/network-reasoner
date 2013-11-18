using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class DimensionalDelegates
	{
		public virtual Func<Vector<NodeForNetwonRaphsonSystem>, Vector<NodeForNetwonRaphsonSystem>> makeAdimensionalToDimensionalTranslator (
			List<NodeForNetwonRaphsonSystem> nodes, GasFormulaVisitor formulae)
		{			
			return (Vector<NodeForNetwonRaphsonSystem> adimensionalUnknowns) => {

				var dimensionalUnknowns = new Vector<NodeForNetwonRaphsonSystem> ();

				nodes.ForEach (aNode => {

					double adimensionalPressure = adimensionalUnknowns.valueAt (aNode);

					double dimensionalPressure = aNode.dimensionalPressureOf (
						adimensionalPressure, formulae);

					dimensionalUnknowns.atPut (aNode, dimensionalPressure);
				}
				);

				return dimensionalUnknowns;
			};
			
		}
		
		public virtual Func<T, T> throwExceptionIfThisTranslatorIsCalled<T> (
			string exceptionMessage)
		{
			return (T aDimensionalValue) => {
				throw new Exception (exceptionMessage);
			};
		}
	}
}

