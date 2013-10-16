using System;
using NUnit.Framework;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.tests
{
	[TestFixture()]
	public class EdgeForNetwonRaphsonSystemCreation
	{
		/********************************************************
		 *	In this tests we don't care about the correct setup
		 *	of original nodes since there will exists tests that
		 *	cover that topic. Nevertheless we should check the
		 *	properties of the constructed node from the original
		 *	ones.
		 *******************************************************/


		[Test()]
		public void creating_switched_on_edge ()
		{
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode =
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			var originalStartNode = new GasNodeTopological ();
			var originalEndNode = new GasNodeTopological ();

			var newtonRaphsonStartNode = new NodeForNetwonRaphsonSystem ();
			var newtonRaphsonEndNode = new NodeForNetwonRaphsonSystem ();

			newtonRaphsonNodesByOriginalNode.Add (originalStartNode, newtonRaphsonStartNode);
			newtonRaphsonNodesByOriginalNode.Add (originalEndNode, newtonRaphsonEndNode);

			var aDiameter = 4873.298;
			long aLength = 8476;
			var aMaxSpeed = 3524.09;
			var aRoughness = 9487.093;

			GasEdgeAbstract aTopologicalEdge = new GasEdgeTopological{ 
				StartNode = originalStartNode, 
				EndNode = originalEndNode};

			GasEdgeAbstract anEdge = new GasEdgePhysical{ 
				Diameter = aDiameter, 
				Length = aLength, 
				MaxSpeed = aMaxSpeed, 
				Roughness = aRoughness,
				Described = aTopologicalEdge};

			var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
			aBuilder.CustomNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

			var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);

			Assert.AreEqual (aDiameter, edgeForNetwonRaphsonSystem.DiameterInMillimeters);
			Assert.AreEqual (aLength, edgeForNetwonRaphsonSystem.Length);
			Assert.AreSame (newtonRaphsonStartNode, edgeForNetwonRaphsonSystem.StartNode);
			Assert.AreSame (newtonRaphsonEndNode, edgeForNetwonRaphsonSystem.EndNode);
			Assert.IsInstanceOf (typeof(EdgeForNetwonRaphsonSystem.EdgeStateOn), 
			                     edgeForNetwonRaphsonSystem.SwitchState);
		}

		[Test()]
		public void creating_switched_off_edge ()
		{
			Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> newtonRaphsonNodesByOriginalNode =
				new Dictionary<GasNodeAbstract, NodeForNetwonRaphsonSystem> ();

			var originalStartNode = new GasNodeTopological ();
			var originalEndNode = new GasNodeTopological ();

			var newtonRaphsonStartNode = new NodeForNetwonRaphsonSystem ();
			var newtonRaphsonEndNode = new NodeForNetwonRaphsonSystem ();

			var aDiameter = 4873.298;
			long aLength = 8476;
			var aMaxSpeed = 3524.09;
			var aRoughness = 9487.093;

			newtonRaphsonNodesByOriginalNode.Add (originalStartNode, newtonRaphsonStartNode);
			newtonRaphsonNodesByOriginalNode.Add (originalEndNode, newtonRaphsonEndNode);



			GasEdgeAbstract aTopologicalEdge = new GasEdgeTopological{ 
				StartNode = originalStartNode, 
				EndNode = originalEndNode};

			GasEdgeAbstract aPhysicalEdge = new GasEdgePhysical{ 
				Diameter = aDiameter, 
				Length = aLength, 
				MaxSpeed = aMaxSpeed, 
				Roughness = aRoughness,
				Described = aTopologicalEdge};

			GasEdgeAbstract switchedOffEdge = new GasEdgeWithGadget{ 
				Equipped = aPhysicalEdge,
				Gadget = new GasEdgeGadgetSwitchOff()};

			var aBuilder = new EdgeForNetwonRaphsonSystemBuilder ();
			aBuilder.CustomNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

			var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (switchedOffEdge);

			Assert.AreEqual (aDiameter, edgeForNetwonRaphsonSystem.DiameterInMillimeters);
			Assert.AreEqual (aLength, edgeForNetwonRaphsonSystem.Length);
			Assert.AreSame (newtonRaphsonStartNode, edgeForNetwonRaphsonSystem.StartNode);
			Assert.AreSame (newtonRaphsonEndNode, edgeForNetwonRaphsonSystem.EndNode);
			Assert.IsInstanceOf (typeof(EdgeForNetwonRaphsonSystem.EdgeStateOff), 
			                     edgeForNetwonRaphsonSystem.SwitchState);
		}


	}
}

