﻿using System;

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

		public void forIsConsequentInPressureRegulation (IsConsequentInPressureRegulation isConsequentInPressureRegulation)
		{
			this.Otherwise.Invoke ();
			return;
			// FIXME: this otherwise branch should be handled in a more first-class manner.
			throw new NotImplementedException ();
		}

		#endregion
	}
}

