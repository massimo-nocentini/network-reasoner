using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystemBuilder : 
	GasEdgeVisitor, GasEdgeGadgetVisitor
	{
		public Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> 
				CustomNodesByGeneralNodes{ get; set; }

		public EdgeForNetwonRaphsonSystem CustomEdgeUnderBuilding{ get; set; }

		#region GasEdgeVisitor implementation

		public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
		{
			CustomEdgeUnderBuilding.DiameterInMillimeters = gasEdgePhysical.Diameter;
			CustomEdgeUnderBuilding.Length = gasEdgePhysical.Length;
			CustomEdgeUnderBuilding.RoughnessInMicron = gasEdgePhysical.Roughness;
			gasEdgePhysical.Described.accept (this);
		}

		public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
		{
			CustomEdgeUnderBuilding.StartNode = 
					CustomNodesByGeneralNodes [gasEdgeTopological.StartNode];

			CustomEdgeUnderBuilding.EndNode = 
					CustomNodesByGeneralNodes [gasEdgeTopological.EndNode];

			CustomEdgeUnderBuilding.Identifier = 
				gasEdgeTopological.Identifier;
		}

		public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
		{
			gasEdgeWithGadget.Gadget.accept (this);

			// here we continue the recursion just for elegance and 
			// foreseeing in the future if we add a gadget for edges that 
			// doesn't switch off the edge.
			gasEdgeWithGadget.Equipped.accept (this);
		}

		#endregion

		#region GasEdgeGadgetVisitor implementation

		public void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
		{
			this.CustomEdgeUnderBuilding.SwitchState = 
				new EdgeForNetwonRaphsonSystem.EdgeStateOff ();
		}

		public void forPressureRegulatorGadget (
			GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator)
		{
			this.CustomEdgeUnderBuilding.RegulatorState = 
				new EdgeForNetwonRaphsonSystem.IsEdgeRegulator ();
		}

		#endregion

		protected virtual EdgeForNetwonRaphsonSystem.EdgeState 
			makeInitialSwitchStateOnEdgeUnderConstruction ()
		{
			return	new EdgeForNetwonRaphsonSystem.EdgeStateOn ();
		}

		protected virtual EdgeForNetwonRaphsonSystem.EdgeRegulator 
			makeInitialRegulatorStateOnEdgeUnderConstruction ()
		{
			return new EdgeForNetwonRaphsonSystem.IsNotEdgeRegulator ();
		}

		public EdgeForNetwonRaphsonSystem buildCustomEdgeFrom (
			GasEdgeAbstract anEdge)
		{
			this.CustomEdgeUnderBuilding = new EdgeForNetwonRaphsonSystem ();

			this.CustomEdgeUnderBuilding.SwitchState = 
				this.makeInitialSwitchStateOnEdgeUnderConstruction ();

			this.CustomEdgeUnderBuilding.RegulatorState = 
				this.makeInitialRegulatorStateOnEdgeUnderConstruction ();

			anEdge.accept (this);

			return this.CustomEdgeUnderBuilding;
		}

	}
}

