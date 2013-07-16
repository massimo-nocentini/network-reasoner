using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class DotRepresentationValidator : ValidatorAbstract
	{
		public String DotCommand{ get; set; }

		public String GeneratedImageOutputFile{ get; set; }

		public String DotRepresentationOutputFile{ get; set; }

		public String ImageEncoding{ get; set; }

		protected virtual void BuildRepresentation (
			GasNetwork gasNetwork, StringBuilder dotRepresentation)
		{
			dotRepresentation.AppendLine ("digraph G {");
			foreach (var edge in gasNetwork.Edges.Values) {
				string start = edge.StartNode.Identifier.Replace (" ", "");
				string end = edge.EndNode.Identifier.Replace (" ", "");
				
				String line = start + " -> " + end;

				dotRepresentation.AppendLine (line);
			}
			dotRepresentation.AppendLine ("}");
		}

		public void validate (GasNetwork gasNetwork)
		{


			StringBuilder dotRepresentation = new StringBuilder ();
			BuildRepresentation (gasNetwork, dotRepresentation);

			this.CreateFileForDotRepresentation (dotRepresentation.ToString ());

			this.InvokeDotCommand (dotRepresentation, OnImageGenerated);
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

