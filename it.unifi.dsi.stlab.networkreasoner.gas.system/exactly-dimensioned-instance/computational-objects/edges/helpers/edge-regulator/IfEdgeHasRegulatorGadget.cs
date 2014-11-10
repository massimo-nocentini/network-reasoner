using System;

namespace  it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges
{
	public class IfEdgeHasRegulatorGadget : EdgeRegulatorVisitor
	{
		public Action Do { get; set; }

		public void performOn (EdgeRegulator regulatorState)
		{
			regulatorState.accept (this);
		}

		#region EdgeRegulatorVisitor implementation

		public void forIsNotEdgeRegulator (IsNotEdgeRegulator isNotEdgeRegulator)
		{
			// since this object has been created in relation
			// to an edge without a regulator gadget, we ignore
			// the action to invoke since it is not
			// our responsibility.
		}

		public void forIsEdgeRegulator (IsEdgeRegulator isEdgeRegulator)
		{
			Do.Invoke ();
		}

		#endregion
	}
}

