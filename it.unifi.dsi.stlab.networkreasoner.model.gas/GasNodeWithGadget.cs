using System;
using it.unifi.dsi.stlab.networkreasoner.systemsolver;

namespace it.unifi.dsi.stlab.networkreasoner.model.gas
{
	public class GasNodeWithGadget : GasNodeAbstract
	{
		public GasNodeAbstract Equipped{ get; set; }

		public GasNodeGadget Gadget{ get; set; }

		public GasNodeWithGadget ()
		{
		}

		public override string Comment {
			get {
				return Equipped.Comment;
			}
			set {
				Equipped.Comment = value;
			}
		}

		public override long Height {
			get {
				return Equipped.Height;
			}
			set {
				Equipped.Height = value;
			}
		}

		public override string Identifier {
			get {
				return Equipped.Identifier;
			}
			set {
				Equipped.Identifier = value;
			}
		}

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

		private class MatrixConstructorWithLoadGadget : NodeMatrixConstruction
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
			public double coefficient ()
			{
				return GasNodeGadgetLoad.Load;
			}
			#endregion

		}

		private class MatrixConstructorWithSupplyGadget : NodeMatrixConstruction
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
			public double coefficient ()
			{
				return GasNodeGadgetSupply.SetupPressure;
			}
			#endregion

		}

	}
}

