using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class IEnumerableExtensionMethods
	{
		public abstract class FetchExacltyOneErrorHandler
		{
			public abstract void moreThanOneElementIsPresent ();

			public abstract void emptyEnumerable ();
		}

		public class FetchExacltyOneErrorHandlerNullObject :	
			FetchExacltyOneErrorHandler
		{
			#region FetchExacltyOneErrorHandler implementation
			public override  void moreThanOneElementIsPresent ()
			{

			}

			public override void emptyEnumerable ()
			{

			}
			#endregion
		}

		public class FetchExacltyOneErrorHandlerThrowExceptionIfErrorOccurs :	
			FetchExacltyOneErrorHandler
		{
			#region FetchExacltyOneErrorHandler implementation
			public override  void moreThanOneElementIsPresent ()
			{
				throw new Exception ("More than one element is present in the enumerable.");
			}

			public override void emptyEnumerable ()
			{
				throw new Exception ("No element in the enumerable.");
			}
			#endregion
		}

		public static T FetchExactlyOne<T> (this IEnumerable<T> anEnumerable)
		{
			return anEnumerable.FetchExactlyOneWithErrorHandler (
				new FetchExacltyOneErrorHandlerNullObject ());
		}

		public static T FetchExactlyOneThrowingExceptionsIfErrorsOccurs<T> (
			this IEnumerable<T> anEnumerable)
		{
			return anEnumerable.FetchExactlyOneWithErrorHandler (
				new FetchExacltyOneErrorHandlerThrowExceptionIfErrorOccurs ());
		}

		public static T FetchExactlyOneWithErrorHandler<T> (
			this IEnumerable<T> anEnumerable, 
			FetchExacltyOneErrorHandler errorHandler)
		{
			var enumerator = anEnumerable.GetEnumerator ();

			if (enumerator.MoveNext () == false) {
				errorHandler.emptyEnumerable ();
			}

			var typeSpecification = enumerator.Current;

			if (enumerator.MoveNext ()) {
				errorHandler.moreThanOneElementIsPresent ();
			}

			return typeSpecification;
		}
	}
}

