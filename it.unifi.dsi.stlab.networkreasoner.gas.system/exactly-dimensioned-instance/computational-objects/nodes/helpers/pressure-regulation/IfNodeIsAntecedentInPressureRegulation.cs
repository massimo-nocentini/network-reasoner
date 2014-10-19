using System;

namespace  it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public class IfNodeIsAntecedentInPressureRegulation : AntecedentInPressureRegulationVisitor
	{
		public Action<IsAntecedentInPressureRegulation> IfItIs{ get; set; }

		public Action Otherwise{ get; set; }

		public IfNodeIsAntecedentInPressureRegulation ()
		{
			this.Otherwise = new Action (DoNothing);
		}

		protected virtual void DoNothing ()
		{
		}

		public void performOn (AntecedentInPressureRegulation roleInPressureRegulation)
		{
			roleInPressureRegulation.accept (this);
		}

		#region AntecedentInPressureRegulationVisitor implementation

		public void forIsAntecedentInPressureRegulation (IsAntecedentInPressureRegulation isAntecedentInPressureRegulation)
		{
			this.IfItIs.Invoke (isAntecedentInPressureRegulation);
		}

		public void forIsNotAntecedentInPressureRegulation (IsNotAntecedentInPressureRegulation isNotAntecedentInPressureRegulation)
		{
			this.Otherwise.Invoke ();
		}

		#endregion
	}
}

