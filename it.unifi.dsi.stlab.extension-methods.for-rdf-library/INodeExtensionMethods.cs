using System;
using VDS.RDF;


namespace it.unifi.dsi.stlab.extension_methods.for_rdf_library
{
	public static class INodeExtensionMethods
	{
		public interface NodeTypeDispatching
		{
			void blankNode (IBlankNode iBlankNode);

			void literalNode (ILiteralNode iLiteralNode);

			void uriNode (IUriNode iUriNode);

			void unknownNode (INode aNode);

		}

		public abstract class NodeTypeDispatchingThrowExceptionForEachCase : 
			NodeTypeDispatching
		{
			private Exception Exception { get; set; }

			public NodeTypeDispatchingThrowExceptionForEachCase (Exception ex)
			{
				this.Exception = ex;
			}

			protected virtual void Throw ()
			{
				throw this.Exception;
			}

			#region NodeTypeDispatching implementation
			public virtual void blankNode (IBlankNode iBlankNode)
			{
				this.Throw ();
			}

			public virtual void literalNode (ILiteralNode iLiteralNode)
			{
				this.Throw ();
			}

			public virtual void uriNode (IUriNode iUriNode)
			{
				this.Throw ();
			}

			public virtual void unknownNode (INode aNode)
			{
				this.Throw ();
			}
			#endregion
		}

		public static void DispatchOnNodeType (this INode aNode, 
		                                       NodeTypeDispatching dispatching)
		{
			if (aNode.NodeType == NodeType.Blank) {
				dispatching.blankNode (aNode as IBlankNode);
			} else if (aNode.NodeType == NodeType.Literal) {
				dispatching.literalNode (aNode as ILiteralNode);
			} else if (aNode.NodeType == NodeType.Uri) {
				dispatching.uriNode (aNode as IUriNode);
			} else {
				dispatching.unknownNode (aNode);
			}
		}
	}
}

