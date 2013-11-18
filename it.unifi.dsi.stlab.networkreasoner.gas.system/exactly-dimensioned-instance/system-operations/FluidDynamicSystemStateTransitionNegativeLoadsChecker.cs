using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.utilities.object_with_substitution;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionNegativeLoadsChecker
		: FluidDynamicSystemStateTransition
	{
		public GasFormulaVisitor FormulaVisitor{ get; set; }

//			internal Func<Vector<NodeForNetwonRaphsonSystem>, Vector<NodeForNetwonRaphsonSystem>> 
//			FromDimensionalToAdimensionalTranslator{ get; set; }

//			public NegativeLoadsCheckerTransitionBuilder useAdimensionalTranslator (
//				Func<Vector<NodeForNetwonRaphsonSystem>, Vector<NodeForNetwonRaphsonSystem>> 
//				fromDimensionalToAdimensionalTranslator
//			)
//			{
//				this.FromDimensionalToAdimensionalTranslator = fromDimensionalToAdimensionalTranslator;
//				return this;
//			}
//

		#region FluidDynamicSystemStateTransition implementation
		public FluidDynamicSystemStateAbstract forBareSystemState (
			FluidDynamicSystemStateBare fluidDynamicSystemStateBare)
		{
			throw new Exception ("A system in the bare state cannot be checked " +
				"for negative loads, initialize and solve it before and try again"
			);
		}

		public FluidDynamicSystemStateAbstract forUnsolvedSystemState (
			FluidDynamicSystemStateUnsolved fluidDynamicSystemStateUnsolved)
		{
			throw new Exception ("A system in the unsolve state cannot be checked " +
				"for negative loads, solve it before and try again"
			);
		}

		public FluidDynamicSystemStateAbstract forMathematicallySolvedState (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved)
		{		

			List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> nodesSubstitions =
				new List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> ();

			List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> edgesSubstitions = 
				new List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> ();

			FluidDynamicSystemStateNegativeLoadsCorrected correctedState;

			recursion (fluidDynamicSystemStateMathematicallySolved,
			           nodesSubstitions,
			           edgesSubstitions,
			           out correctedState);

			return correctedState;

		}

		protected virtual void recursion (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved,
			List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> nodesSubstitutions,
			List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> edgesSubstitutions,
			out FluidDynamicSystemStateNegativeLoadsCorrected correctedState)
		{
			var previousMutationResults = fluidDynamicSystemStateMathematicallySolved.MutationResult;

			var relativeUnknownsDimensionalWrapper = this.makeUnknownsDimensional (
				previousMutationResults.Unknowns, 
				previousMutationResults.StartingUnsolvedState);

			var relativeUnknowns = relativeUnknownsDimensionalWrapper.WrappedObject;

			var nodeWithMinValue = relativeUnknowns.findKeyWithMinValue ();

			// here we don't know if the pressure is negative or not.
			var originalNode = previousMutationResults.
				StartingUnsolvedState.OriginalNodesByComputationNodes [nodeWithMinValue];

			var pressure = relativeUnknowns.valueAt (nodeWithMinValue);

			if (pressure < 0) {

				GasNodeAbstract substitutedNode = 
					nodeWithMinValue.substituteNodeBecauseNegativePressureFound (
						pressure, originalNode);

				// we keep note that a new node has been created
				nodesSubstitutions.Add (
					new ObjectWithSubstitutionInSameType<GasNodeAbstract>{
					Original = originalNode,
					Substituted = substitutedNode
				}
				);

				List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> currentEdgeSubstitutions;
				GasNetwork networkWithFixedNodesWithLoadGadget = 
						previousMutationResults.StartingUnsolvedState.
						OriginalNetwork.makeFromRemapping (
							nodesSubstitutions,
							out currentEdgeSubstitutions);

				currentEdgeSubstitutions.ForEach (aCurrentEdgeSubstitution => {

					ObjectWithSubstitutionInSameType<GasEdgeAbstract> transitiveSubstitutedNode = 
						edgesSubstitutions.Find (aGivenEdgeSubstitution => 
						aGivenEdgeSubstitution.Substituted.Equals (
								aCurrentEdgeSubstitution.Original)					
					);

					if (transitiveSubstitutedNode != null) {
						transitiveSubstitutedNode.Substituted = 
							aCurrentEdgeSubstitution.Substituted;
					} else {
						edgesSubstitutions.Add (aCurrentEdgeSubstitution);
					}
				}
				);

				var initializerTransition = previousMutationResults.
					StartingUnsolvedState.InitializedBy.clone () as FluidDynamicSystemStateTransitionInitialization;
				initializerTransition.Network = networkWithFixedNodesWithLoadGadget;

				var newtonRaphsonSolverTransition = fluidDynamicSystemStateMathematicallySolved.SolvedBy;

				var newBareSystemState = new FluidDynamicSystemStateBare ();
				var newUnsolvedSystemState = newBareSystemState.doStateTransition (
					initializerTransition);
				var newMathematicallySolvedSystemState = newUnsolvedSystemState.doStateTransition (
					newtonRaphsonSolverTransition);

				recursion (newMathematicallySolvedSystemState as FluidDynamicSystemStateMathematicallySolved,
				          nodesSubstitutions,
				          edgesSubstitutions,
				          out correctedState);
			} else {
				correctedState = new FluidDynamicSystemStateNegativeLoadsCorrected ();
				correctedState.FluidDynamicSystemStateMathematicallySolved = 
					fluidDynamicSystemStateMathematicallySolved;
				correctedState.NodesSubstitutions = nodesSubstitutions;
				correctedState.EdgesSubstitutions = edgesSubstitutions;
				correctedState.CorrectedBy = this;
			}

		}

		public FluidDynamicSystemStateAbstract forNegativeLoadsCorrectedState (
			FluidDynamicSystemStateNegativeLoadsCorrected fluidDynamicSystemStateNegativeLoadsCorrected)
		{
			// simply return the given state since if it is already checked we've to do nothing here
			return fluidDynamicSystemStateNegativeLoadsCorrected;
		}

		public FluidDynamicSystemStateTransition clone ()
		{
			throw new System.NotImplementedException ();
		}
		
		#endregion

		protected virtual DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> makeUnknownsDimensional (
			DimensionalObjectWrapper<Vector<NodeForNetwonRaphsonSystem>> adimensionalWrapper,
			FluidDynamicSystemStateUnsolved FluidDynamicSystemStateUnsolved)
		{
			var translator = new DimensionalDelegates ().makeAdimensionalToDimensionalTranslator (
				FluidDynamicSystemStateUnsolved.Nodes,
				FormulaVisitor);

			var dimensionalUnknowns = adimensionalWrapper.makeDimensional (translator);

			return dimensionalUnknowns;
		}
	}
}

