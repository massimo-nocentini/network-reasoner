using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.times_of_computation;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class ListExtensionMethods
	{
		public class ListItemDecoratedWithTimeComputation<T>
		{
			public T Item{ get; set; }

			public TimeOfComputationHandling ComputationTime{ get; set; }
		}

		public static List<T> Rest<T> (this List<T> aList)
		{
			return aList.IsEmpty () ? aList : aList.GetRange (1, aList.Count - 1);
		}

		public static bool IsEmpty<T> (this List<T> aList)
		{
			return aList.Count == 0;
		}

		public static T First<T> (this List<T> aList)
		{
			return aList [0];
		}

		public static List<ListItemDecoratedWithTimeComputation<T>> 
			DecoreWithTimeComputation<T> (this List<T> aList)
		{
			var decored = new List<ListItemDecoratedWithTimeComputation<T>> ();

			if (aList.IsEmpty ()) {
				return decored;
			}

			decored.Add (new ListItemDecoratedWithTimeComputation<T> {
				Item = aList.First (),
				ComputationTime = new TimeOfComputationHandlingFirstOne ()
			}
			);

			aList.Rest ().ForEach (anItem => decored.Add (
				new ListItemDecoratedWithTimeComputation<T> {
				Item = anItem,
				ComputationTime = new TimeOfComputationHandlingBeyondFirst ()
			}
			)
			);

			return decored;
		}
	}
}

