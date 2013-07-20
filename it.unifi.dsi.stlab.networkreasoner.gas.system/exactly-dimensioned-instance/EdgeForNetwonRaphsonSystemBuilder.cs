using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance
{
	internal class EdgeForNetwonRaphsonSystemBuilder : 
			GasEdgeVisitor, GasEdgeGadgetVisitor
		{
			interface EdgeState
			{
				void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector);
			}

			class EdgeStateOn:EdgeState
			{
				#region EdgeState implementation
				public void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector)
				{
					collector.Add (aBuilder.CustomEdgeUnderBuilding);
				}
				#endregion
			}

			class EdgeStateOff:EdgeState
			{
				#region EdgeState implementation
				public void buildCustomEdgeFor (
					EdgeForNetwonRaphsonSystemBuilder aBuilder,
					List<EdgeForNetwonRaphsonSystem> collector)
				{
					// here we simple do not add the edge since it is switched off.
				}
				#endregion
			}

			public Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> 
				customNodesByGeneralNodes{ get; set; }

			public AmbientParameters AmbientParameters{ get; set; }

			public EdgeForNetwonRaphsonSystem CustomEdgeUnderBuilding{ get; set; }

			EdgeState EdgeIsAllowed{ get; set; }

			public EdgeForNetwonRaphsonSystemBuilder ()
			{
				// here we build the edge for the Netwon-Raphson system.
				this.CustomEdgeUnderBuilding = new EdgeForNetwonRaphsonSystem ();
				this.EdgeIsAllowed = new EdgeStateOn ();
			}

			#region GasEdgeVisitor implementation
			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				CustomEdgeUnderBuilding.Diameter = gasEdgePhysical.Diameter;
				CustomEdgeUnderBuilding.Length = gasEdgePhysical.Length;
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
				this.EdgeIsAllowed = new EdgeStateOff ();
			}
			#endregion

			public void buildCustomEdgeFrom (
				GasEdgeAbstract anEdge, List<EdgeForNetwonRaphsonSystem> collector)
			{
				anEdge.accept (this);
				this.CustomEdgeUnderBuilding.AmbientParameters = AmbientParameters;

				this.EdgeIsAllowed.buildCustomEdgeFor (this, collector);
			}

		}
}

