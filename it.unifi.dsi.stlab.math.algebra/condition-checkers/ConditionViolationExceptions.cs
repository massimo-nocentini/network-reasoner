using System;

namespace it.unifi.dsi.stlab.math.algebra
{
	public abstract class MissingKeyAbstractException<IndexType> : Exception
	{
		public IndexType MissingIndex{ get; set; }
	}

	public class RightVectorHasMissingIndexException<IndexType>:MissingKeyAbstractException<IndexType>
	{

	}

	public class LeftVectorHasMissingIndexException<IndexType>:MissingKeyAbstractException<IndexType>
	{

	}

	public class IndexNotCoveredByContextException<IndexType> : Exception
	{
		public IndexType IndexNotCovered{ get; set; }
	}
}

