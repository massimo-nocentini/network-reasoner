using System;
using MathNet.Numerics.LinearAlgebra.Double;
using log4net;
using System.Globalization;

namespace it.unifi.dsi.stlab.extensionmethods
{
	public static class VectorExtensionMethods
	{
		public static void writeIntoLog (this Vector aVector, ILog aLog, string formatMessage)
		{
			var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone ();
			formatProvider.TextInfo.ListSeparator = " ";

			aLog.DebugFormat (formatMessage, 
			                  aVector.ToString ("#0.00\n", formatProvider));


		}
	}
}

