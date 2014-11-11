using System;

namespace  it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public class IfTrueIfFalseEdgeHasRegulatorGadget : EdgeRegulatorVisitor
	{
		public Action IfTrue { get; set; }

		public Action IfFalse { get; set; }

		public void performOn (EdgeRegulator regulatorState)
		{
			regulatorState.accept (this);
		}

		#region EdgeRegulatorVisitor implementation

		public void forIsNotEdgeRegulator (IsNotEdgeRegulator isNotEdgeRegulator)
		{
			this.IfFalse.Invoke ();
		}

		public void forIsEdgeRegulator (IsEdgeRegulator isEdgeRegulator)
		{
			this.IfTrue.Invoke ();
		}

		#endregion
	}

}

