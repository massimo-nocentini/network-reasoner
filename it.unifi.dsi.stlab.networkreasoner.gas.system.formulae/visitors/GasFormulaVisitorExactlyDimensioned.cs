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
		public virtual Double visitCoefficientFormulaForNodeWithSupplyGadget (
			CoefficientFormulaForNodeWithSupplyGadget aSupplyNodeFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aSupplyNodeFormula);

			var numerator = aSupplyNodeFormula.GadgetSetupPressureInMillibar / 1000d + 
				AirPressureInBar;

			var denominator = AmbientParameters.RefPressureInBar;

			var Hsetup = Math.Pow (numerator / denominator, 2d);
		
			return Hsetup;
		}

		public double visitAirPressureFormulaForNodes (
			AirPressureFormulaForNodes anAirPressureFormula)
		{
			double airPressureInBar = AmbientParameters.AirPressureInBar * 
				Math.Exp (-(AmbientParameters.GravitationalAcceleration * anAirPressureFormula.NodeHeight) / 
				(AmbientParameters.AirRconstant () * AmbientParameters.AirTemperatureInKelvin)
			);

			return airPressureInBar;
		}

		public virtual double visitRelativePressureFromAdimensionalPressureFormulaForNodes (
			RelativePressureFromAdimensionalPressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var AirPressureInBar = this.computeAirPressureFromHeightHolder (
				aRelativePressureFromAbsolutePressureFormula);

			var result = Math.Sqrt (aRelativePressureFromAbsolutePressureFormula.AbsolutePressure) *
				AmbientParameters.RefPressureInBar;

			return (result - AirPressureInBar) * 1e3;
		}

		public virtual double visitAbsolutePressureFromAdimensionalPressureFormulaForNodes (
			AbsolutePressureFromAdimensionalPressureFormulaForNodes aRelativePressureFromAbsolutePressureFormula)
		{
			var result = Math.Sqrt (aRelativePressureFromAbsolutePressureFormula.AbsolutePressure) *
				AmbientParameters.RefPressureInBar;

			return result;
		}

		public virtual double visitCovariantLittleKFormula (
			CovariantLittleKFormula covariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant () + 
				this.weightedHeightsDifferenceFor (covariantLittleKFormula);
		}

		public virtual double visitControVariantLittleKFormula (
			ControVariantLittleKFormula controVariantLittleKFormula)
		{
			return this.AmbientParameters.Rconstant () - 
				this.weightedHeightsDifferenceFor (controVariantLittleKFormula);
		}

		public double visitKvalueFormula (KvalueFormula aKvalueFormula)
		{		

			var f = aKvalueFormula.EdgeFvalue;

			var A = this.AmbientParameters.Aconstant () / 
				Math.Pow (aKvalueFormula.EdgeDiameterInMillimeters / 1000d, 5d);

			var unknownForStartNode = aKvalueFormula.UnknownForEdgeStartNode;
			var unknownForEndNode = aKvalueFormula.UnknownForEdgeEndNode;
				
			var weightedHeightsDifference = Math.Abs (
				aKvalueFormula.EdgeCovariantLittleK * unknownForStartNode - 
				aKvalueFormula.EdgeControVariantLittleK * unknownForEndNode
			);

			var length = aKvalueFormula.EdgeLength;

			var K = 3600d / Math.Sqrt (f * A * length * weightedHeightsDifference);

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

			double initialValue = 0d;

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

			double initialValue = 0d;

			AmatrixQuadruplet result = new AmatrixQuadruplet ();

			result.StartNodeStartNodeUpdater = cumulate => -coVariant / 2d + cumulate;
			result.StartNodeStartNodeInitialValue = initialValue;

			result.StartNodeEndNodeUpdater = cumulate => controVariant / 2d + cumulate;
			result.StartNodeEndNodeInitialValue = initialValue;

			result.EndNodeStartNodeUpdater = cumulate => coVariant / 2d + cumulate;
			result.EndNodeStartNodeInitialValue = initialValue;

			result.EndNodeEndNodeUpdater = cumulate => -controVariant / 2d + cumulate;
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

			double numeratorForRe = Math.Abs ((FvalueFormula.EdgeQvalue * 4d) / 3600d) * 
				this.AmbientParameters.RefDensity ();

			var denominatorForRe = Math.PI * (FvalueFormula.EdgeDiameterInMillimeters / 1000d) * 
				this.AmbientParameters.ViscosityInPascalTimesSecond;

			var Re = numeratorForRe / denominatorForRe;

			var augend = FvalueFormula.EdgeRoughnessInMicron / 
				(FvalueFormula.EdgeDiameterInMillimeters * 1000d * 3.71);

			var addend = 2.51d / (Re * Math.Sqrt (FvalueFormula.EdgeFvalue));

			var toInvert = -2d * Math.Log10 (augend + addend);

			var Fvalue = Math.Pow (1d / toInvert, 2d);
			
			if (Re < 2000d) {
				Fvalue = 64d / Re;	
			}
			else if(Re < 4000d) {
				Fvalue = .00277 * Math.Pow(Re, .3219);	
			}

			return Fvalue;
		}

		public double visitVelocityValueFormula (VelocityValueFormula velocityValueFormula)
		{
			double Qvalue = velocityValueFormula.Qvalue;
			double diameter = velocityValueFormula.Diameter;
			double absolutePressureOfStartNode = velocityValueFormula.AbsolutePressureOfStartNode;
			double absolutePressureOfEndNode = velocityValueFormula.AbsolutePressureOfEndNode;
			
			var minPressure = Math.Min (absolutePressureOfStartNode, absolutePressureOfEndNode);

			var numerator = (Qvalue / 3600d) * AmbientParameters.RefPressureInBar / minPressure;
			var denominator = Math.PI * Math.Pow (diameter * 1e-3, 2d) / 4d; 

			return numerator / denominator;
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
			var airPressureInBar = airPressureFormula.accept (this);

			return airPressureInBar;
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

