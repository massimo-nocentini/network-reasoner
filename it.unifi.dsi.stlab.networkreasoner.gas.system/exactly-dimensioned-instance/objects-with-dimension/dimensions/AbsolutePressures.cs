using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class AbsolutePressures : TargetDimension<Vector<NodeForNetwonRaphsonSystem>>
	{
		public List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public GasFormulaVisitor Formulae { get; set; }

		#region Dimension implementation
		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateRelativeValues (
			DimensionalObjectWrapperWithRelativeValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithRelativeValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAbsoluteValues (
			DimensionalObjectWrapperWithAbsoluteValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAbsoluteValues)
		{
			return dimensionalObjectWrapperWithAbsoluteValues;
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAdimensionalValues (
			DimensionalObjectWrapperWithAdimensionalValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAdimensionalValues)
		{
			var absolutePressures = new DimensionalObjectWrapperWithAbsoluteValues<
				Vector<NodeForNetwonRaphsonSystem>> ();

			absolutePressures.WrappedObject = new Vector<NodeForNetwonRaphsonSystem> ();
			
			Nodes.ForEach (aNode => {

				double adimensionalPressure = dimensionalObjectWrapperWithAdimensionalValues.WrappedObject.valueAt (aNode);

				double absolutePressure = aNode.absoluteDimensionalPressureOf (
						adimensionalPressure, Formulae);

				absolutePressures.WrappedObject.atPut (aNode, absolutePressure);
			}
			);

			return absolutePressures;
		}
		#endregion
	}
}

