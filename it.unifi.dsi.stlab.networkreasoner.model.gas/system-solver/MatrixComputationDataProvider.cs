using System;

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
		#region MatrixComputationDataProvider implementation
		public double LittleK (NodeMatrixConstruction startNode, 
		                       NodeMatrixConstruction endNode)
		{
			throw new System.NotImplementedException ();
		}

		public double BigK (NodeMatrixConstruction startNode, 
		                    NodeMatrixConstruction endNode)
		{
			throw new System.NotImplementedException ();
		}
		#endregion
	}
}

