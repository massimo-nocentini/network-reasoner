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

		protected virtual void BuildRepresentation (
			List<EdgeForDotRepresentationValidator> edges, 
			StringBuilder dotRepresentation)
		{
			// we draw an undirected graph
			dotRepresentation.AppendLine ("graph G {");
			foreach (var edge in edges) {
				dotRepresentation.AppendLine (edge.dotRepresentation ());
			}
			dotRepresentation.AppendLine ("}");
		}

		protected interface EdgeState
		{
			string dotRepresentationFor (EdgeForDotRepresentationValidator anEdge);
		}

		class EdgeStateOn : EdgeState
		{
			#region EdgeState implementation
			public string dotRepresentationFor (
				EdgeForDotRepresentationValidator anEdge)
			{
				string start = anEdge.StartNode.Identifier.Replace (" ", "");
				string end = anEdge.EndNode.Identifier.Replace (" ", "");
				
				return start + " -- " + end;
			}
			#endregion
		}

		class EdgeStateOff : EdgeState
		{
			#region EdgeState implementation
			public string dotRepresentationFor (EdgeForDotRepresentationValidator anEdge)
			{
				string start = anEdge.StartNode.Identifier.Replace (" ", "");
				string end = anEdge.EndNode.Identifier.Replace (" ", "");

				// append as suffix the grey color in order to distinguish
				// the edge that is switched off.
				return start + " -- " + end;
			}
			#endregion
		}

		protected class NodeForDotRepresentationValidator : GasNodeVisitor
		{
			public String Identifier { get; set; }

			#region GasNodeVisitor implementation
			public void forNodeWithTopologicalInfo (GasNodeTopological gasNodeTopological)
			{
				this.Identifier = gasNodeTopological.Identifier;
			}

			public void forNodeWithGadget (GasNodeWithGadget gasNodeWithGadget)
			{
				// for now we don't care to represent in a different way
				// the vertex with a supply gadget respect to vertex with
				// a load gadget.
				gasNodeWithGadget.Equipped.accept (this);
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
			#endregion

			public string dotRepresentation ()
			{
				return this.State.dotRepresentationFor (this);
			}


		}

		public void validate (GasNetwork gasNetwork)
		{
			List<EdgeForDotRepresentationValidator> edges;
			this.setupValidatorFor (gasNetwork, out edges);

			StringBuilder dotRepresentation = new StringBuilder ();
			BuildRepresentation (edges, dotRepresentation);

			this.CreateFileForDotRepresentation (dotRepresentation.ToString ());

			this.InvokeDotCommand (dotRepresentation, OnImageGenerated);
		}

		void setupValidatorFor (
			GasNetwork gasNetwork, 
			out List<EdgeForDotRepresentationValidator> edges)
		{
			var result = new List<EdgeForDotRepresentationValidator> ();

			gasNetwork.doOnEdges (new GasNetwork.NodeHandlerWithDelegateOnKeyedNode<
			                      GasEdgeAbstract> (
				(key, anEdge) => {

				var newEdge = new EdgeForDotRepresentationValidator{ 
					State = new EdgeStateOn()};

				anEdge.accept (newEdge);

				result.Add (newEdge);
			}
			)
			);

			edges = result;
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

