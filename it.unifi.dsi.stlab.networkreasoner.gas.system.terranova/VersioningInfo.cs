using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.terranova
{
	public class VersioningInfo
	{
		/// <summary>
		/// Gets the version number.
		/// 
		/// Here are the stack of versions with a brief description:
		/// 
		/// v1.1.0: introduce pressure regulator gadget for edges.
		/// 
		/// v1.0.1: enhancement to tackle turbolent behavior due to 
		/// 		Reynolds number.
		/// 
		/// v1.0.0: basic version with checker about negative pressures
		/// 		associated to nodes with load gadget.
		/// 
		/// </summary>
		/// <value>The version number.</value>
		public String VersionNumber { 
			get {
				return "v1.1.0";
			}
		}

		public String EngineIdentifier { 
			get {
				return "Network Reasoner, stlab.dsi.unifi.it";
			}
		}
	}
}

