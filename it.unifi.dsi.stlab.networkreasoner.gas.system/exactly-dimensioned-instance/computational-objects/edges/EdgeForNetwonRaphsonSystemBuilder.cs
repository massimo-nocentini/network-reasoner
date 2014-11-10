using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.edges;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance.computational_objects.nodes;

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
			CustomEdgeUnderBuilding.StartNode.OutgoingEdges.Add (this.CustomEdgeUnderBuilding);

			CustomEdgeUnderBuilding.EndNode = 
					CustomNodesByGeneralNodes [gasEdgeTopological.EndNode];
			CustomEdgeUnderBuilding.EndNode.IncomingEdges.Add (this.CustomEdgeUnderBuilding);

			CustomEdgeUnderBuilding.Identifier = 
				gasEdgeTopological.Identifier;
		}

		public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
		{
			// Observation: here the order of the following two recursion calls
			// does matter a lot because with the former one we'll go deep and
			// set the start and end node...
			gasEdgeWithGadget.Equipped.accept (this);

			// instead, with the latter one we apply the gadget information
			// to possibly the start and end nodes (like pressure regulator gadget)
			gasEdgeWithGadget.Gadget.accept (this);
		}

		#endregion

		#region GasEdgeGadgetVisitor implementation

		public void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
		{
			this.CustomEdgeUnderBuilding.SwitchState = 
				new EdgeStateOff ();
		}

		public void forPressureRegulatorGadget (
			GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator)
		{
			this.CustomEdgeUnderBuilding.RegulatorState = 
				new IsEdgeRegulator ();

			this.CustomEdgeUnderBuilding.StartNode.RoleInPressureRegulation = 
				new IsAntecedentInPressureRegulation { 
				Regulator = this.CustomEdgeUnderBuilding.EndNode
			};

			this.CustomEdgeUnderBuilding.EndNode.RoleInPressureRegulation = 
				new IsConsequentInPressureRegulation {
				Antecedent = this.CustomEdgeUnderBuilding.StartNode
			};
		}

		#endregion

		protected virtual EdgeState 
			makeInitialSwitchStateOnEdgeUnderConstruction ()
		{
			return	new EdgeStateOn ();
		}

		protected virtual EdgeRegulator 
			makeInitialRegulatorStateOnEdgeUnderConstruction ()
		{
			return new IsNotEdgeRegulator ();
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

