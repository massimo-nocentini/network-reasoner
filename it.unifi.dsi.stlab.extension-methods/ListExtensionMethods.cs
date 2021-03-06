using System;
using System.Collections.Generic;
using it.unifi.dsi.stlab.utilities.times_of_computation;
using it.unifi.dsi.stlab.utilities.object_with_substitution;

namespace it.unifi.dsi.stlab.extension_methods
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

		public static Dictionary<T, T> OriginalsBySubstituted<T> (
			this List<ObjectWithSubstitutionInSameType<T>> aList)
		{
			Dictionary<T, T> originalsBySubstituted = 
				new Dictionary<T, T> ();

			aList.ForEach (anObjWithSubstitution => 
			               originalsBySubstituted.Add (anObjWithSubstitution.Substituted, 
				anObjWithSubstitution.Original)
			);

			return originalsBySubstituted;
		}

		public static Dictionary<T, T> SubstitutedByOriginals<T> (
			this List<ObjectWithSubstitutionInSameType<T>> aList)
		{
			Dictionary<T, T> substitutedByOriginals = 
				new Dictionary<T, T> ();

			aList.ForEach (anObjWithSubstitution => 
			               substitutedByOriginals.Add (anObjWithSubstitution.Original, 
				anObjWithSubstitution.Substituted)
			);

			return substitutedByOriginals;
		}

		public static void FindIfFoundIfNot<T> (
			this List<T> aList, 
			Predicate<T> predicate,
			Action<T> ifFoundDo,
			Action ifNotFoundDo)
		{
			T element = aList.Find (predicate);

			if (element != null) {
				ifFoundDo.Invoke (element);
			} else {
				ifNotFoundDo.Invoke ();
			}
		}

		public static void ForEachFindInDo<T, S> (
			this List<T> domainList, 
			List<S> lookingAtList, 
			Predicate<Tuple<T, S>> predicate,
			Action<Tuple<T, S>> ifFoundDo,
			Action<T> ifNotFoundDo)
		{ 
			domainList.ForEach (domainElement => {

				lookingAtList.FindIfFoundIfNot (
					aGivenEdgeSubstitution => predicate.Invoke (
						Tuple.Create (domainElement, aGivenEdgeSubstitution)),
					foundElement => ifFoundDo.Invoke (Tuple.Create (domainElement, foundElement)),
					() => ifNotFoundDo.Invoke (domainElement));
			});
		}
	}
}

