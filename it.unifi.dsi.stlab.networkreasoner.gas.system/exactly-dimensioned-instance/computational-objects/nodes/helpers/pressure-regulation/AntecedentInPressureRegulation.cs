using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes
{
	public interface AntecedentInPressureRegulation
	{
		void accept (AntecedentInPressureRegulationVisitor aVisitor);
	}

	public interface AntecedentInPressureRegulationVisitor
	{
		void forIsAntecedentInPressureRegulation (IsAntecedentInPressureRegulation isAntecedentInPressureRegulation);

		void forIsNotAntecedentInPressureRegulation (IsNotAntecedentInPressureRegulation isNotAntecedentInPressureRegulation);

		void forIsConsequentInPressureRegulation (IsConsequentInPressureRegulation isConsequentInPressureRegulation);
	}

	public class IsAntecedentInPressureRegulation : AntecedentInPressureRegulation
	{
		public NodeForNetwonRaphsonSystem Regulator{ get; set; }

		#region AntecedentInPressureRegulation implementation

		public void accept (AntecedentInPressureRegulationVisitor aVisitor)
		{
			aVisitor.forIsAntecedentInPressureRegulation (this);
		}

		#endregion
	}

	public class IsConsequentInPressureRegulation : AntecedentInPressureRegulation
	{
		public NodeForNetwonRaphsonSystem Antecedent{ get; set; }

		#region AntecedentInPressureRegulation implementation

		public void accept (AntecedentInPressureRegulationVisitor aVisitor)
		{
			aVisitor.forIsConsequentInPressureRegulation (this);
		}

		#endregion
	}

	public class IsNotAntecedentInPressureRegulation : AntecedentInPressureRegulation
	{
		#region AntecedentInPressureRegulation implementation

		public void accept (AntecedentInPressureRegulationVisitor aVisitor)
		{
			aVisitor.forIsNotAntecedentInPressureRegulation (this);
		}

		#endregion


	}
}

