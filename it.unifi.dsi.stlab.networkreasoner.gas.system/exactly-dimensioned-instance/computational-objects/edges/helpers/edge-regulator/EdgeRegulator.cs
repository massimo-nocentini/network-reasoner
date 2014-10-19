using System;

namespace  it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public interface EdgeRegulatorVisitor
	{
		void forIsNotEdgeRegulator (IsNotEdgeRegulator isNotEdgeRegulator);

		void forIsEdgeRegulator (IsEdgeRegulator isEdgeRegulator);
	}

	public interface EdgeRegulator
	{
		void accept (EdgeRegulatorVisitor aVisitor);
	}

	public class IsEdgeRegulator : EdgeRegulator
	{
		#region EdgeRegulator implementation

		public void accept (EdgeRegulatorVisitor aVisitor)
		{
			aVisitor.forIsEdgeRegulator (this);
		}

		#endregion
	}

	public class IsNotEdgeRegulator : EdgeRegulator
	{
		#region EdgeRegulator implementation

		public void accept (EdgeRegulatorVisitor aVisitor)
		{
			aVisitor.forIsNotEdgeRegulator (this);
		}

		#endregion
	}
}

