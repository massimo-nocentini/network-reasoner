using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class UnknownInitializationSimplyRandomized : UnknownInitialization
	{
		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.UnknownInitialization
		public override double initialValueFor (GasNodeAbstract aVertex, Random rand)
		{
			return (rand.NextDouble () * .1) + 1;
		}
		#endregion

	}
}

