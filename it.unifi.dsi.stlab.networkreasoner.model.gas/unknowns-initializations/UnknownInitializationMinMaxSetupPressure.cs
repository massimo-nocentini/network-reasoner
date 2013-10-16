using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class UnknownInitializationMinMaxSetupPressure : UnknownInitialization
	{

		public class MaxMinSetupPressureIntervalFinder : GasNodeVisitor, GasNodeGadgetVisitor
		{
			public AmbientParameters AmbientParameters{ get; set; }

			Double? MaxSeenSetupPressure{ get; set; }

			Double? MinSeenSetupPressure{ get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				// nothing to do since no gadget found for this variant.
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}
			#endregion

			#region GasNodeGadgetVisitor implementation
			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				// nothing to do since the gadget is a load.
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				if (MaxSeenSetupPressure.HasValue) {
					if (aSupplyGadget.SetupPressure > MaxSeenSetupPressure.Value) {
						MaxSeenSetupPressure = aSupplyGadget.SetupPressure;
					}
				} else {
					MaxSeenSetupPressure = aSupplyGadget.SetupPressure;
				}

				if (MinSeenSetupPressure.HasValue) {
					if (aSupplyGadget.SetupPressure < MinSeenSetupPressure.Value) {
						MinSeenSetupPressure = aSupplyGadget.SetupPressure;
					}
				} else {
					MinSeenSetupPressure = aSupplyGadget.SetupPressure;
				}
			}
			#endregion

			public double adimensionalizedMinMax (Func<double, double, double> continuation)
			{
				// per l'acqua si deve correggere come fatto nelle formule.

				double min = (this.MinSeenSetupPressure.Value / 1000.0 + 
					this.AmbientParameters.AirPressureInBar) /
					this.AmbientParameters.RefPressureInBar;

				double max = (this.MaxSeenSetupPressure.Value / 1000.0 + 
					this.AmbientParameters.AirPressureInBar) /
					this.AmbientParameters.RefPressureInBar;

				return continuation.Invoke (Math.Pow (min, 2), Math.Pow (max, 2));
			}
		}

		Double FixingTerm{ get; set; }

		MaxMinSetupPressureIntervalFinder MinMaxIntervalFinder{ get; set; }

		public static UnknownInitialization make (
			GasNetwork aGasNetwork,
			AmbientParameters ambientParameters)
		{
			MaxMinSetupPressureIntervalFinder minMaxIntervalFinder = 
			new MaxMinSetupPressureIntervalFinder {
				AmbientParameters = ambientParameters
			};

			aGasNetwork.doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => aVertex.accept (minMaxIntervalFinder))
			);

			return new UnknownInitializationMinMaxSetupPressure{
				FixingTerm = .97,
				MinMaxIntervalFinder = minMaxIntervalFinder
			};
		}

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.UnknownInitialization
		public override double initialValueFor (GasNodeAbstract aVertex, Random rand)
		{
			return this.MinMaxIntervalFinder.adimensionalizedMinMax (
				(min, max) => {
				var maxMinusMin = max - (min * this.FixingTerm);
				var value = (min * this.FixingTerm) + (maxMinusMin * rand.NextDouble ());
				return value;
			}
			);
		}
		#endregion

	}
}

