using System;
using log4net;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class MatrixExtensionMethods
	{
		public static void writeIntoLog (this Matrix aMatrix, ILog aLog, String formatMessage)
		{
			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone ();
			formatProvider.TextInfo.ListSeparator = " ";

			aLog.DebugFormat (formatMessage, 
			                  aMatrix.ToString ("#0.00\n", formatProvider));
		}
	}
}

