using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface NetworkIteratorHandler<T>
	{
		void onRawNode (T aNode);

		void onNodeWithKey (string aNodeIdentifier, T aNode);
	}

	public abstract class NetworkIteratorHandlerAbstract<T> : NetworkIteratorHandler<T>
	{
			#region NodeHandler implementation
		public virtual void onRawNode (T aNode)
		{

		}

		public virtual void onNodeWithKey (
				string aNodeIdentifier, T aNode)
		{

		}
			#endregion
	}

	public class NodeHandlerWithDelegateOnRawNode<T> : 
		NetworkIteratorHandlerAbstract<T>
	{
		Action<T> aBlock { get; set; }

		public NodeHandlerWithDelegateOnRawNode (Action<T> aBlock)
		{
			this.aBlock = aBlock;
		}

			#region NodeHandler implementation
		public override void onRawNode (T aNode)
		{
			this.aBlock.Invoke (aNode);
		}
			#endregion

	}

	public class NodeHandlerWithDelegateOnKeyedNode<T> : 
		NetworkIteratorHandlerAbstract<T>
	{
		Action<String, T> aBlock { get; set; }

		public NodeHandlerWithDelegateOnKeyedNode (
				Action<String, T> aBlock)
		{
			this.aBlock = aBlock;
		}

		public override void onNodeWithKey (
				string aNodeIdentifier, T aNode)
		{
			this.aBlock.Invoke (aNodeIdentifier, aNode);
		}
	}
}

