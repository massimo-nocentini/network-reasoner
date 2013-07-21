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
		[Test()]
		public void TestCase ()
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
			aBuilder.customNodesByGeneralNodes = newtonRaphsonNodesByOriginalNode;

			var edgeForNetwonRaphsonSystem = aBuilder.buildCustomEdgeFrom (anEdge);

			Assert.AreEqual (aDiameter, edgeForNetwonRaphsonSystem.Diameter);
			Assert.AreEqual (aLength, edgeForNetwonRaphsonSystem.Length);
			Assert.AreSame (newtonRaphsonStartNode, edgeForNetwonRaphsonSystem.StartNode);
			Assert.AreSame (newtonRaphsonEndNode, edgeForNetwonRaphsonSystem.EndNode);
			Assert.IsInstanceOf (typeof(EdgeForNetwonRaphsonSystem.EdgeStateOn), edgeForNetwonRaphsonSystem.SwitchState);
		}
	}
}

