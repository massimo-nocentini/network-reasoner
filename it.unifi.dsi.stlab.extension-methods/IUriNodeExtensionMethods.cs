using System;
using VDS.RDF;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class IUriNodeExtensionMethods
	{
		public static Object GetValue (this IUriNode node, Func<Uri, Object> mapper)
		{
			return mapper.Invoke (node.Uri);
		}
	}
}

