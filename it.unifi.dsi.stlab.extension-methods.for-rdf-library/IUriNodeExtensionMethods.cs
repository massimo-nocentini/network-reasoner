using System;
using VDS.RDF;

namespace it.unifi.dsi.stlab.extension_methods.for_rdf_library
{
	public static class IUriNodeExtensionMethods
	{
		public static Object GetValue (this IUriNode node, Func<Uri, Object> mapper)
		{
			return mapper.Invoke (node.Uri);
		}
	}
}

