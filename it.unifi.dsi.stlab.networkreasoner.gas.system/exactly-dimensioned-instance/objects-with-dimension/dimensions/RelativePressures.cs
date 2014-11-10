using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.math.algebra;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects
{
	public class RelativePressures : TargetDimension<Vector<NodeForNetwonRaphsonSystem>>
	{
		public List<NodeForNetwonRaphsonSystem> Nodes{ get; set; }

		public GasFormulaVisitor Formulae { get; set; }

		#region Dimension implementation

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateRelativeValues (DimensionalObjectWrapperWithRelativeValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithRelativeValues)
		{
			return dimensionalObjectWrapperWithRelativeValues;
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAbsoluteValues (DimensionalObjectWrapperWithAbsoluteValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAbsoluteValues)
		{
			throw new System.NotImplementedException ();
		}

		public DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> translateAdimensionalValues (DimensionalObjectWrapperWithAdimensionalValues<Vector<NodeForNetwonRaphsonSystem>> dimensionalObjectWrapperWithAdimensionalValues)
		{
			var relativePressures = new DimensionalObjectWrapperWithRelativeValues<
				Vector<NodeForNetwonRaphsonSystem>> ();

			relativePressures.WrappedObject = new Vector<NodeForNetwonRaphsonSystem> ();
			
			Nodes.ForEach (aNode => {

				double adimensionalPressure = dimensionalObjectWrapperWithAdimensionalValues.WrappedObject.valueAt (aNode);

				double relativePressure = aNode.relativeDimensionalPressureOf (
					                          adimensionalPressure, Formulae);

				relativePressures.WrappedObject.atPut (aNode, relativePressure);
			}
			);

			return relativePressures;
		}

		#endregion

	}
}

