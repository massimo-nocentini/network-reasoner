using System;
using System.Text;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public abstract class SummaryTableItem
	{
		public abstract void appendValueInto (StringBuilder table);

		public abstract void appendHeaderInto (StringBuilder table);

		public abstract void appendIdentifierForSingleRunAnalysisInto (StringBuilder table);

		public String Identifier{ get; set; }

		public int Position{ get; set; }

		protected virtual String formatDouble (Double value)
		{
			return Math.Abs (value) >= 1e6 ? value.ToString ("E3") : value.ToString ("0.000");
		}
			
		protected virtual string columnSeparator ()
		{
			// using this column separator we aim to create an emacs-readable table
			return "|";
		}
	}
}

