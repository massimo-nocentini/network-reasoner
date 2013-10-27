using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.textualinterface
{
	public class NodeSpecificationLine
	{
		public string Identifier {
			get;
			set;
		}

		public long Height {
			get;
			set;
		}

		public NodeType Type {
			get;
			set;
		}

		public string[] SplittedSpecification {
			get;
			set;
		}



	}

}

