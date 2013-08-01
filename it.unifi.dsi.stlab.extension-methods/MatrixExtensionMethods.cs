using System;
using log4net;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class MatrixExtensionMethods
	{
		public static void stringRepresentation (
			this Matrix aMatrix, Action<String> continuation)
		{
			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone ();
			formatProvider.TextInfo.ListSeparator = " ";

			var representation = aMatrix.ToString ("#0.00\n", formatProvider);

			continuation.Invoke (representation);
		}
	}
}

