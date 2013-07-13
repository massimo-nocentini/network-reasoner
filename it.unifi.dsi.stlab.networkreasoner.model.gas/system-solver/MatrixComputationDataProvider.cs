using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface MatrixComputationDataProvider
	{
		double LittleK (NodeMatrixConstruction startNode, 
		              NodeMatrixConstruction endNode);
	}
}

