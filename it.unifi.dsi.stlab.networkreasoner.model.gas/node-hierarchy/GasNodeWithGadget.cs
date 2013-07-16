using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeWithGadget : GasNodeAbstract
	{
		public GasNodeAbstract Equipped{ get; set; }

		public GasNodeGadget Gadget{ get; set; }	

		#region implemented abstract members of it.unifi.dsi.stlab.networkreasoner.model.gas.GasNodeAbstract
		public override NodeMatrixConstruction adapterForMatrixConstruction ()
		{
			return Gadget.dispatchForNodeMatrixConstructionOn (this);
		}
		#endregion	

		public NodeMatrixConstruction makeNodeMatrixConstructionForLoadGadget (
			GasNodeGadgetLoad gasNodeGadgetLoad)
		{
			return new MatrixConstructorWithLoadGadget (this, gasNodeGadgetLoad);
		}

		public NodeMatrixConstruction makeNodeMatrixConstructionForSupplyGadget (
			GasNodeGadgetSupply gasNodeGadgetSupply)
		{
			return new MatrixConstructorWithSupplyGadget (this, gasNodeGadgetSupply);
		}

		private abstract class AbstractNodeMatrixConstruction : NodeMatrixConstruction
		{
			#region NodeMatrixConstruction implementation
			public abstract double coefficient ();

			public double matrixValueRespect (NodeMatrixConstruction anotherNode, 
			                                  MatrixComputationDataProvider dataProvider)
			{
				return anotherNode.dispatchOnIdentity (this, dataProvider);
			}

			public double dispatchOnIdentity (NodeMatrixConstruction aNode, 
			                                  MatrixComputationDataProvider dataProvider)
			{
				double result = 0;
				if (Object.ReferenceEquals (this, aNode)) {
					result = aNode.matrixValueForYourColumn (aNode, dataProvider);
				} else {
					result = aNode.matrixValueForNotYourColumn (aNode, dataProvider);
				}

				return result;
			}

			public abstract double matrixValueForYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider);

			public abstract double matrixValueForNotYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider);

			#endregion
		}

		private class MatrixConstructorWithLoadGadget : AbstractNodeMatrixConstruction
		{
			private GasNodeWithGadget GasNodeWithGadget{ get; set; }

			private GasNodeGadgetLoad GasNodeGadgetLoad{ get; set; }

			public MatrixConstructorWithLoadGadget (
				GasNodeWithGadget gasNodeWithGadget, 
				GasNodeGadgetLoad gasNodeGadgetLoad)
			{
				this.GasNodeGadgetLoad = gasNodeGadgetLoad;
				this.GasNodeWithGadget = gasNodeWithGadget;
			}

			#region NodeMatrixConstruction implementation
			public override double coefficient ()
			{
				return GasNodeGadgetLoad.Load;
			}

			public override double matrixValueForYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider)
			{
				double bigK = dataProvider.BigK (this, aNode);
				double littleK = dataProvider.LittleK (this, aNode);
				return  bigK * littleK;
			}

			public override double matrixValueForNotYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider)
			{
				double bigK = dataProvider.BigK (this, aNode);
				double littleK = dataProvider.LittleK (aNode, this);
				return  bigK * (-1 * littleK);
			}
			#endregion





		}

		private class MatrixConstructorWithSupplyGadget : AbstractNodeMatrixConstruction
		{
			private GasNodeWithGadget GasNodeWithGadget{ get; set; }

			private GasNodeGadgetSupply GasNodeGadgetSupply{ get; set; }

			public MatrixConstructorWithSupplyGadget (
				GasNodeWithGadget gasNodeWithGadget, 
				GasNodeGadgetSupply gasNodeGadgetSupply)
			{
				this.GasNodeGadgetSupply = gasNodeGadgetSupply;
				this.GasNodeWithGadget = gasNodeWithGadget;
			}

			#region NodeMatrixConstruction implementation
			public override double coefficient ()
			{
				return GasNodeGadgetSupply.SetupPressure;
			}

			public override double matrixValueForYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider)
			{
				return 1;
			}

			public override double matrixValueForNotYourColumn (
				NodeMatrixConstruction aNode, 
				MatrixComputationDataProvider dataProvider)
			{
				return 0;
			}
			#endregion


		}

	}
}

