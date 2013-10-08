using System;
using it.unifi.dsi.stlab.networkreasoner.model.gas;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system.formulae
{
	public class GasFormulaVisitorExactlyDimensioned : GasFormulaVisitor
	{
		// for ease the implementation we assume that the ambiental
		// parameters are passed from the outside and stored here
		// as an instance variable.
		public AmbientParameters AmbientParameters{ get; set; }

		#region GasFormulaVisitor implementation
		public Double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aSupplyNodeFormula);

			var numerator = aSupplyNodeFormula.GadgetSetupPressureInMillibar / 1000 + 
				AirPressureInBar;

			var denominator = AmbientParameters.RefPressureInBar;

			var Hsetup = Math.Pow (numerator / denominator, 2);
		
			return Hsetup;
		}

		// for water
		public Double visitCoefficientFormulaForNodeWithSupplyGadgetH2O (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aSupplyNodeFormula);

//			var specWeight = AmbientParameters.WaterRefDensity * AmbientParameters.GravitationalAcceleration;

			var specWeight = 1.0; // dummy value just to make it compile.

			// per l'acqua sara' in bar aSupplyNodeFormula.GadgetSetupPressureInMillibar
			var numerator = (AirPressureInBar + 
				aSupplyNodeFormula.GadgetSetupPressureInMillibar) * 1e5 // per andare in pascal
				/ specWeight;

			var result = numerator + aSupplyNodeFormula.NodeHeight;
		
			return result;
		}

		public double visitAirPressureFormulaForNodes (
			AirPressureFormulaForNodes anAirPressureFormula)
		{
			var airPressureInBar = AmbientParameters.AirPressureInBar * 
				Math.Exp (-(AmbientParameters.GravitationalAcceleration * anAirPressureFormula.NodeHeight) / 
				(AmbientParameters.Rconstant() * AmbientParameters.AirTemperatureInKelvin)
			);

			return airPressureInBar;
		}

		// denormalizzazione acqua 
		public double visitRelativePressureFromAbsolutePressureFormulaForNodesH2O (
			RelativePressureFromAbsolutePressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aRelativePressureFromAbsolutePressureFormula);

			var z = aRelativePressureFromAbsolutePressureFormula.AbsolutePressure;
			var h = aRelativePressureFromAbsolutePressureFormula.NodeHeight;

			// Default AmbientParameters.WaterRefDensity := 1000 kg/m^3
//			var specWeight = AmbientParameters.WaterRefDensity * 
//				AmbientParameters.GravitationalAcceleration;

			var specWeight = 1.0; // dummy value just to make it compile.

			var result = (z - h) * specWeight * 1e-5;

			return (result - AirPressureInBar);
		}

		// denormalizzazione gas 
		public double visitRelativePressureFromAbsolutePressureFormulaForNodes (
			RelativePressureFromAbsolutePressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aRelativePressureFromAbsolutePressureFormula);

			var result = Math.Sqrt (aRelativePressureFromAbsolutePressureFormula.AbsolutePressure) *
				AmbientParameters.RefPressureInBar;

			return (result - AirPressureInBar) * 1000;
		}

		public double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant() + 
				this.weightedHeightsDifferenceFor (covariantLittleKFormula);
		}

		public double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant() - 
				this.weightedHeightsDifferenceFor (controVariantLittleKFormula);
		}

		public double visitKvalueFormula (KvalueFormula aKvalueFormula)
		{		

			var f = aKvalueFormula.EdgeFvalue;

			var A = this.AmbientParameters.Aconstant() / 
				Math.Pow (aKvalueFormula.EdgeDiameterInMillimeters / 1000, 5);

			var unknownForStartNode = aKvalueFormula.UnknownForEdgeStartNode;
			var unknownForEndNode = aKvalueFormula.UnknownForEdgeEndNode;
				
			var weightedHeightsDifference = Math.Abs (
				aKvalueFormula.EdgeCovariantLittleK * unknownForStartNode - 
				aKvalueFormula.EdgeControVariantLittleK * unknownForEndNode
			);

			var length = aKvalueFormula.EdgeLength;

			var K = 3600 / Math.Sqrt (f * A * length * weightedHeightsDifference);

			return K;
		}

		public AmatrixQuadruplet visitAmatrixQuadrupletFormulaForSwitchedOnEdges (
			AmatrixQuadrupletFormulaForSwitchedOnEdges AmatrixQuadrupletFormulaForSwitchedOnEdges)
		{
			double coVariant;

			double controVariant;

			computeCovariantAndControVariantFromKvalueHolder (
				AmatrixQuadrupletFormulaForSwitchedOnEdges,
				out coVariant, out controVariant);

			double initialValue = 0.0;

			AmatrixQuadruplet result = new AmatrixQuadruplet ();

			result.StartNodeStartNodeUpdater = cumulate => -coVariant + cumulate;
			result.StartNodeStartNodeInitialValue = initialValue;

			result.StartNodeEndNodeUpdater = cumulate => controVariant + cumulate;
			result.StartNodeEndNodeInitialValue = initialValue;

			result.EndNodeStartNodeUpdater = cumulate => coVariant + cumulate;
			result.EndNodeStartNodeInitialValue = initialValue;

			result.EndNodeEndNodeUpdater = cumulate => -controVariant + cumulate;
			result.EndNodeEndNodeInitialValue = initialValue;

			return result;
		}

		public AmatrixQuadruplet visitJacobianMatrixQuadrupletFormulaForSwitchedOnEdges (
			JacobianMatrixQuadrupletFormulaForSwitchedOnEdges 
			jacobianMatrixQuadrupletFormulaForSwitchedOnEdges
		)
		{
			double coVariant;

			double controVariant;

			computeCovariantAndControVariantFromKvalueHolder (
				jacobianMatrixQuadrupletFormulaForSwitchedOnEdges,
				out coVariant, out controVariant);

			double initialValue = 0.0;

			AmatrixQuadruplet result = new AmatrixQuadruplet ();

			result.StartNodeStartNodeUpdater = cumulate => -coVariant / 2 + cumulate;
			result.StartNodeStartNodeInitialValue = initialValue;

			result.StartNodeEndNodeUpdater = cumulate => controVariant / 2 + cumulate;
			result.StartNodeEndNodeInitialValue = initialValue;

			result.EndNodeStartNodeUpdater = cumulate => coVariant / 2 + cumulate;
			result.EndNodeStartNodeInitialValue = initialValue;

			result.EndNodeEndNodeUpdater = cumulate => -controVariant / 2 + cumulate;
			result.EndNodeEndNodeInitialValue = initialValue;

			return result;
		}

		public double visitQvalueFormula (
			QvalueFormula QvalueFormula)
		{
			var weightedUnknownsDifference = 
					QvalueFormula.EdgeCovariantLittleK * QvalueFormula.UnknownForEdgeStartNode -
				QvalueFormula.EdgeControVariantLittleK * QvalueFormula.UnknownForEdgeEndNode;

			return QvalueFormula.EdgeKvalue * weightedUnknownsDifference;		
		}

		public double visitFvalueFormula (FvalueFormula FvalueFormula)
		{			

			double numeratorForRe = Math.Abs ((FvalueFormula.EdgeQvalue * 4) / 3600) * 
				this.AmbientParameters.RefDensity ();

			var denominatorForRe = Math.PI * (FvalueFormula.EdgeDiameterInMillimeters / 1000) * 
				this.AmbientParameters.ViscosityInPascalTimesSecond;

			var Re = numeratorForRe / denominatorForRe;

			var augend = FvalueFormula.EdgeRoughnessInMicron / 
				(FvalueFormula.EdgeDiameterInMillimeters * 1000 * 3.71);

			var addend = 2.51 / (Re * Math.Sqrt (FvalueFormula.EdgeFvalue));

			var toInvert = -2 * Math.Log10 (augend + addend);

			var Fvalue = Math.Pow (1 / toInvert, 2);

			return Fvalue;
		}
		#endregion

		#region Utility methods, most of them allow behavior factorization.
		protected virtual double weightedHeightsDifferenceFor (
				AbstractLittleKFormula abstractLittleKFormula)
		{
			var difference = abstractLittleKFormula.HeightOfStartNode - 
				abstractLittleKFormula.HeightOfEndNode;

			var rate = this.AmbientParameters.GravitationalAcceleration / 
				this.AmbientParameters.ElementTemperatureInKelvin;

			return rate * difference;

		}

		protected virtual double computeAirPressureFromHeightHolder (
			NodeHeightHolder nodeHeightHolder)
		{
			var airPressureFormula = new AirPressureFormulaForNodes ();
			airPressureFormula.NodeHeight = nodeHeightHolder.NodeHeight;
			var AirPressureInBar = airPressureFormula.accept (this);

			return AirPressureInBar;
		}

		protected virtual void computeCovariantAndControVariantFromKvalueHolder (
			KvalueAndLittleKHolder holder, 
			out double coVariant, 
			out double controVariant)
		{
			coVariant = holder.EdgeKvalue * 
				holder.EdgeCovariantLittleK;

			controVariant = holder.EdgeKvalue * 
				holder.EdgeControVariantLittleK;
		}

		#endregion

	}
}

