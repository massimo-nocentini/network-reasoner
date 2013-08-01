using System;
using MathNet.Numerics.LinearAlgebra.Double;
using log4net;
using System.Globalization;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class VectorExtensionMethods
	{
		public static void stringRepresentation (
			this Vector aVector, Action<String> continuation)
		{
			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone ();
			formatProvider.TextInfo.ListSeparator = " ";

			var representation = aVector.ToString ("#0.00\n", formatProvider);

			continuation.Invoke (representation);
		}
	}
}

