using System;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	/// <summary>
	/// Node height holder.
	/// 
	/// This interface allow to send the message to get the node height in order 
	/// to factor the behavior for the computation of the AirPressure formula, which
	/// depends on nodes' heights. This air pressure value is necessary to formulae that
	/// don't belong to a common hierarchy, hence the interface and not an abstract class.
	/// </summary>
	public interface NodeHeightHolder
	{
		long NodeHeight {
			get;
			set;
		}
	}
}

