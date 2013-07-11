using System;

namespace it.unifi.dsi.stlab.networkreasoner.model
{
	public class LogicalPropertyIdentifier<IdentifierType> : LogicalProperty
	{
		protected IdentifierType identifier;

		public LogicalPropertyIdentifier (IdentifierType identifier)
		{
			this.identifier = identifier;
		}
	}
}

