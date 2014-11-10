using System;

namespace  it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public class IfNodeIsConsequentInPressureRegulation : AntecedentInPressureRegulationVisitor
	{
		public Action<IsConsequentInPressureRegulation> IfItIs{ get; set; }

		public Action Otherwise{ get; set; }

		public IfNodeIsConsequentInPressureRegulation ()
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
			this.Otherwise.Invoke ();
			return;
			// FIXME: this piece of code should be abstracted out in order to have a better casing.
			throw new NotSupportedException ("This method should not be called on a IsAntecedentInPressureRelation");
		}

		public void forIsNotAntecedentInPressureRegulation (IsNotAntecedentInPressureRegulation isNotAntecedentInPressureRegulation)
		{
			this.Otherwise.Invoke ();
		}


		public void forIsConsequentInPressureRegulation (IsConsequentInPressureRegulation isConsequentInPressureRegulation)
		{
			this.IfItIs.Invoke (isConsequentInPressureRegulation);
		}

		#endregion
	}
}

