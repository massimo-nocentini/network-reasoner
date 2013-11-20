using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public interface TableHeaderParser
	{
		List<string> parse (List<string> region);
	}
}

