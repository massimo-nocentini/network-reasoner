using System;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace it.unifi.dsi.stlab.extension_methods.for_rdf_library
{
	public static class ILiteralNodeExtensionMethods
	{
		public static Object GetValue (this ILiteralNode node)
		{
			if (node.DataType == null) {
				return node.AsValuedNode ().AsString ();
			}

			String nodeTypeAsString = node.DataType.Fragment.Replace ("#", "");
			if (nodeTypeAsString.Equals ("double")) {
				return node.AsValuedNode ().AsDouble ();
			} else if (nodeTypeAsString.Equals ("integer")) {
				return node.AsValuedNode ().AsInteger ();			
			} else if (nodeTypeAsString.Equals ("boolean")) {
				return node.AsValuedNode ().AsBoolean ();
			}

			throw new ArgumentException (string.Format (
				"the given literal node has a type {0} which isn't used in this application.",
				nodeTypeAsString)
			);
		}
	}
}

