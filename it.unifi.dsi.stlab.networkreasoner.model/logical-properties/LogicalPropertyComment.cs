using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model
{
	public class LogicalPropertyComment<CommentType> : LogicalProperty
	{
		protected List<CommentType> comment;

		public LogicalPropertyComment (List<CommentType> comment)
		{
			this.comment = comment;
		}
	}
}

