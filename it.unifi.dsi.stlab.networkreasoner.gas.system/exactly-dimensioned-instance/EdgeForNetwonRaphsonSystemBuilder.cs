using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	public class EdgeForNetwonRaphsonSystemBuilder : 
			GasEdgeVisitor, GasEdgeGadgetVisitor
	{
		public Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> 
				customNodesByGeneralNodes{ get; set; }

		public AmbientParameters AmbientParameters{ get; set; }

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
					customNodesByGeneralNodes [gasEdgeTopological.StartNode];

			CustomEdgeUnderBuilding.EndNode = 
					customNodesByGeneralNodes [gasEdgeTopological.EndNode];
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
			#endregion

		protected virtual EdgeForNetwonRaphsonSystem.EdgeState 
			makeInitialSwitchStateOnEdgeUnderConstruction ()
		{
			return	new EdgeForNetwonRaphsonSystem.EdgeStateOn ();
		}

		public EdgeForNetwonRaphsonSystem buildCustomEdgeFrom (
				GasEdgeAbstract anEdge)
		{
			this.CustomEdgeUnderBuilding = new EdgeForNetwonRaphsonSystem ();
			this.CustomEdgeUnderBuilding.SwitchState = 
				this.makeInitialSwitchStateOnEdgeUnderConstruction ();

			anEdge.accept (this);

			// the following line on the ambient parameters will disappear when
			// we'll introduce the Formulae hierarchy.
			this.CustomEdgeUnderBuilding.AmbientParameters = AmbientParameters;

			return this.CustomEdgeUnderBuilding;
		}

	}
}

