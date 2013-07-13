using System;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface MatrixComputationDataProvider
	{
		double LittleK (NodeMatrixConstruction startNode, 
		              NodeMatrixConstruction endNode);

		double BigK (NodeMatrixConstruction startNode, 
		              NodeMatrixConstruction endNode);
	}

	public class MatrixComputationDataProviderDictionaryImplementation :
	MatrixComputationDataProvider
	{
		private Dictionary<KeyValuePair<
			NodeMatrixConstruction, NodeMatrixConstruction>, double> _LittleK { get; set; }

		private Dictionary<KeyValuePair<
			NodeMatrixConstruction, NodeMatrixConstruction>, double> _BigK { get; set; }

		public MatrixComputationDataProviderDictionaryImplementation (
			Dictionary<KeyValuePair<
				NodeMatrixConstruction, NodeMatrixConstruction>, double> littleK, 
			Dictionary<KeyValuePair<
				NodeMatrixConstruction, NodeMatrixConstruction>, double> bigK
		)
		{
			this._LittleK = littleK;
			this._BigK = bigK;
		}

		#region MatrixComputationDataProvider implementation
		public double LittleK (NodeMatrixConstruction startNode, 
		                       NodeMatrixConstruction endNode)
		{
			var key = new KeyValuePair<
				NodeMatrixConstruction, NodeMatrixConstruction> (startNode, endNode);
			return this._LittleK [key];
		}

		public double BigK (NodeMatrixConstruction startNode, 
		                    NodeMatrixConstruction endNode)
		{
			var key = new KeyValuePair<
				NodeMatrixConstruction, NodeMatrixConstruction> (startNode, endNode);
			return this._BigK [key];
		}
		#endregion
	}
}

