using System;
using VDS.RDF;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface
{
	public class NamespaceRepository
	{
		private static readonly String csharp = 
			"http://stlab.dsi.unifi.it/networkreasoner/csharp-types/";
		private static readonly String rdf = 
			"http://www.w3.org/1999/02/22-rdf-syntax-ns#";

		public static Uri rdf_type ()
		{
			return UriFactory.Create (rdf + "type");
		}

		public static Uri csharp_qualified_fullname ()
		{
			return UriFactory.Create (csharp + "qualified-fullname");
		}

	}
}

