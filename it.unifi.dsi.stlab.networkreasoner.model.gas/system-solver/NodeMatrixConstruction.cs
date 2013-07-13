using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public interface NodeMatrixConstruction
	{
		double coefficient ();

		double matrixValueRespect (NodeMatrixConstruction anotherNode, 
		                           MatrixComputationDataProvider dataProvider);

		double dispatchOnIdentity (NodeMatrixConstruction aNode,
		                         MatrixComputationDataProvider dataProvider);

		double matrixValueForYourColumn (NodeMatrixConstruction aNode,
		                             MatrixComputationDataProvider dataProvider);

		double matrixValueForNotYourColumn (NodeMatrixConstruction aNode,
		                             MatrixComputationDataProvider dataProvider);



	}


}

