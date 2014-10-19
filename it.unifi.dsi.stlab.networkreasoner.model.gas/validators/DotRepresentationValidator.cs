using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class DotRepresentationValidator : ValidatorAbstract
	{
		public String DotCommand{ get; set; }

		public String GeneratedImageOutputFile{ get; set; }

		public String DotRepresentationOutputFile{ get; set; }

		public String ImageEncoding{ get; set; }

		void BuildRepresentationForVertices (
			List<NodeForDotRepresentationValidator> vertices, 
			StringBuilder dotRepresentation)
		{
			foreach (var vertex in vertices) {
				dotRepresentation.AppendLine (vertex.dotRepresentation ());
			}
		}

		protected virtual void BuildRepresentationForEdges (
			List<EdgeForDotRepresentationValidator> edges, 
			StringBuilder dotRepresentation)
		{
			foreach (var edge in edges) {
				dotRepresentation.AppendLine (edge.dotRepresentation ());
			}
		}

		protected interface EdgeState
		{
			string propertyForEdge (EdgeForDotRepresentationValidator anEdge);
		}

		class EdgeStateOn : EdgeState
		{
			#region EdgeState implementation
			public string propertyForEdge (
				EdgeForDotRepresentationValidator anEdge)
			{
				return "";
			}
			#endregion
		}

		class EdgeStateOff : EdgeState
		{
			#region EdgeState implementation
			public string propertyForEdge (
				EdgeForDotRepresentationValidator anEdge)
			{
				return "color=\"gray\"";
			}
			#endregion
		}

		protected class NodeForDotRepresentationValidator : 
		GasNodeVisitor, GasNodeGadgetVisitor
		{
			interface VertexRole
			{
				string propertyFor (
					NodeForDotRepresentationValidator nodeForDotRepresentationValidator);
			}

			class VertexRoleSupplier : VertexRole
			{
				#region VertexRole implementation
				public string propertyFor (
					NodeForDotRepresentationValidator nodeForDotRepresentationValidator)
				{
					return "fillcolor=yellow, style=\"rounded,filled\", shape=diamond";
				}
				#endregion
			}

			class VertexRoleLoader : VertexRole
			{
				#region VertexRole implementation
				public string propertyFor (
					NodeForDotRepresentationValidator nodeForDotRepresentationValidator)
				{
					return "";
				}
				#endregion
			}

			// by default we assume that all vertices have a load gadget,
			// recursion will fix this assumption if necessary due to the
			// input structure of the vertex.
			VertexRole aVertexRole = new VertexRoleLoader ();

			public String Identifier { get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (
				GasNodeTopological gasNodeTopological)
			{
				this.Identifier = gasNodeTopological.Identifier;
			}

			public void forNodeWithGadget (
				GasNodeWithGadget gasNodeWithGadget)
			{
				gasNodeWithGadget.Gadget.accept (this);
				gasNodeWithGadget.Equipped.accept (this);
			}

			public void forNodeAntecedentInPressureReduction (
				GasNodeAntecedentInPressureRegulator gasNodeAntecedentInPressureRegulator)
			{
				// simply forward to the top node, hence in the drawing
				// we wont see any difference for a node antecedent in a 
				// pressure regulator relation.
				gasNodeAntecedentInPressureRegulator.ToppedNode.accept (this);
			}
			#endregion
			public string dotRepresentation ()
			{
				return this.Identifier.Replace (" ", "") +
					" [" + aVertexRole.propertyFor (this) + "];";
			}	
			#region GasNodeGadgetVisitor implementation
			public void forLoadGadget (GasNodeGadgetLoad aLoadGadget)
			{
				this.aVertexRole = new VertexRoleLoader ();
			}

			public void forSupplyGadget (GasNodeGadgetSupply aSupplyGadget)
			{
				this.aVertexRole = new VertexRoleSupplier ();
			}
			#endregion


		}

		protected class EdgeForDotRepresentationValidator : 
			GasEdgeVisitor,
		GasEdgeGadgetVisitor
		{
			public EdgeState State{ get; set; }

			public NodeForDotRepresentationValidator StartNode { get; set; }

			public NodeForDotRepresentationValidator EndNode { get; set; }

			#region GasEdgeVisitor implementation
			public void forPhysicalEdge (GasEdgePhysical gasEdgePhysical)
			{
				gasEdgePhysical.Described.accept (this);
			}

			public void forTopologicalEdge (GasEdgeTopological gasEdgeTopological)
			{
				this.StartNode = new NodeForDotRepresentationValidator ();
				gasEdgeTopological.StartNode.accept (this.StartNode);

				this.EndNode = new NodeForDotRepresentationValidator ();
				gasEdgeTopological.EndNode.accept (this.EndNode);
			}

			public void forEdgeWithGadget (GasEdgeWithGadget gasEdgeWithGadget)
			{
				gasEdgeWithGadget.Gadget.accept (this);
				gasEdgeWithGadget.Equipped.accept (this);
			}
			#endregion		

			#region GasEdgeGadgetVisitor implementation
			public void forSwitchOffGadget (GasEdgeGadgetSwitchOff gasEdgeGadgetSwitchOff)
			{
				this.State = new EdgeStateOff ();
			}

			public void forPressureRegulatorGadget (GasEdgeGadgetPressureRegulator gasEdgeGadgetPressureRegulator)
			{
				// nothing for now, we don't catch this information, so 
				// the line representing this edge cannot be distinguished
				// by the following ones.
			}
			#endregion



			public string dotRepresentation ()
			{
				string start = this.StartNode.Identifier.Replace (" ", "");
				string end = this.EndNode.Identifier.Replace (" ", "");

				// append as suffix the grey color in order to distinguish
				// the edge that is switched off.
				return start + " -- " + end + " [" + this.State.propertyForEdge (this) + "];";
			}


		}

		public StringBuilder generateContent (GasNetwork gasNetwork)
		{
			List<EdgeForDotRepresentationValidator> edges;
			List<NodeForDotRepresentationValidator> vertices;
			this.setupValidatorFor (gasNetwork, out edges, out vertices);

			StringBuilder dotRepresentation = new StringBuilder ();

			// we draw an undirected graph
			dotRepresentation.AppendLine ("graph G {");
			dotRepresentation.AppendLine ("edge [arrowsize=.5, weight=.1, color=\"black\", fontsize=8];");
			//dotRepresentation.AppendLine ("node [label=\"\",shape=circle,height=0.12,width=0.12,fontsize=1]");
			BuildRepresentationForVertices (vertices, dotRepresentation);
			BuildRepresentationForEdges (edges, dotRepresentation);
			dotRepresentation.AppendLine ("}");

			return dotRepresentation;
		}

		public void validate (GasNetwork gasNetwork)
		{
			StringBuilder dotRepresentation = generateContent (gasNetwork);

			this.CreateFileForDotRepresentation (dotRepresentation.ToString ());

			this.InvokeDotCommand (dotRepresentation, OnImageGenerated);
		}

		void setupValidatorFor (
			GasNetwork gasNetwork, 
			out List<EdgeForDotRepresentationValidator> edges,
			out List<NodeForDotRepresentationValidator> vertices)
		{
			var resultEdges = new List<EdgeForDotRepresentationValidator> ();

			gasNetwork.doOnEdges (new NodeHandlerWithDelegateOnKeyedNode<
			                      GasEdgeAbstract> (
				(key, anEdge) => {

				var newEdge = new EdgeForDotRepresentationValidator{ 
					State = new EdgeStateOn()};

				anEdge.accept (newEdge);

				resultEdges.Add (newEdge);
			}
			)
			);

			var resultVertices = new List<NodeForDotRepresentationValidator> ();
			gasNetwork.doOnNodes (new NodeHandlerWithDelegateOnRawNode<GasNodeAbstract> (
				aVertex => {

				var newVertex = new NodeForDotRepresentationValidator ();

				aVertex.accept (newVertex);

				resultVertices.Add (newVertex);
			}
			)
			);

			edges = resultEdges;
			vertices = resultVertices;
		}

		protected virtual void CreateFileForDotRepresentation (string content)
		{
			File.WriteAllText (DotRepresentationOutputFile, content);
		}

		protected virtual void OnImageGenerated (String generatedImage)
		{
			File.WriteAllText (GeneratedImageOutputFile, generatedImage);
		}

		protected virtual void InvokeDotCommand (
			StringBuilder dotRepresentation, Action<String> onImageGenerated)
		{
			var processStartInfo = new ProcessStartInfo ();
			processStartInfo.FileName = this.DotCommand;
			processStartInfo.UseShellExecute = false;
			processStartInfo.Arguments = string.Format ("-T{0} {1}", 
			                                this.ImageEncoding,
			                                Path.GetFullPath (DotRepresentationOutputFile));
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.WorkingDirectory = Directory.GetCurrentDirectory ();

			using (Process process = Process.Start(processStartInfo)) {

				process.WaitForExit ();
				using (StreamReader reader = process.StandardOutput) {

					string generatedImage = reader.ReadToEnd ();

					onImageGenerated.Invoke (generatedImage);
				}
			}

		}


	}
}

