using System;
using VDS.RDF.Parsing;
using VDS.RDF;
using System.IO;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface
{
	public class SpecificationLoader
	{

		private IRdfReader Reader{ get; set; }

		private SpecificationLoader (IRdfReader reader)
		{
			this.Reader = reader;
		}

		public static SpecificationLoader make_nturtle_specification_loader ()
		{
			return new SpecificationLoader (new TurtleParser ());
		}

		public void LoadFile (string filename)
		{
			IGraph g = new Graph ();

			// TODO: catch possible exceptions as explained in the 
			// web document.
			this.Reader.Load (g, filename);

			foreach (Triple triple in g.Triples) {
				String asString = triple.ToString ();
			}
		}
	}
}

