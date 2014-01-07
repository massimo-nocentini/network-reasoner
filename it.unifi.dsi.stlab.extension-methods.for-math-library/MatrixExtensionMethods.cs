using System;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.extension_methods.for_math_library
{
	public static class MatrixExtensionMethods
	{
		public static void stringRepresentation (
			this Matrix aMatrix, Action<String> continuation)
		{
			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone ();
			formatProvider.TextInfo.ListSeparator = " ";

			var representation = aMatrix.ToMatrixString (aMatrix.RowCount, aMatrix.ColumnCount, formatProvider);

			continuation.Invoke (representation);
		}
	}
}

