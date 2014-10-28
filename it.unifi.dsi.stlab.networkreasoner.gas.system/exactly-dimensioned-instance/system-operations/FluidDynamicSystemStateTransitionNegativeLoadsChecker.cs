using System;
using it.unifi.dsi.stlab.networkreasoner.gas.system.dimensional_objects;
using it.unifi.dsi.stlab.math.algebra;
using it.unifi.dsi.stlab.networkreasoner.gas.system.exactly_dimensioned_instance;
using System.Collections.Generic;
using it.unifi.dsi.stlab.networkreasoner.gas.system.formulae;
using it.unifi.dsi.stlab.networkreasoner.model.gas;
using it.unifi.dsi.stlab.utilities.object_with_substitution;
using it.unifi.dsi.stlab.extension_methods;

namespace it.unifi.dsi.stlab.networkreasoner.gas.system
{
	public class FluidDynamicSystemStateTransitionNegativeLoadsChecker
		: FluidDynamicSystemStateTransition
	{

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

			var previousMutationResults = fluidDynamicSystemStateMathematicallySolved.MutationResult;
			var startTimestampOfPreviousResults = previousMutationResults.ComputationStartTimestamp;
			var iterationNumberOfPreviousResults = previousMutationResults.IterationNumber;

			recursion (fluidDynamicSystemStateMathematicallySolved,
				nodesSubstitions,
				edgesSubstitions,
				startTimestampOfPreviousResults,
				iterationNumberOfPreviousResults,
				out correctedState);

			return correctedState;

		}

		protected virtual void recursion (
			FluidDynamicSystemStateMathematicallySolved fluidDynamicSystemStateMathematicallySolved,
			List<ObjectWithSubstitutionInSameType<GasNodeAbstract>> nodesSubstitutions,
			List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> edgesSubstitutions,
			DateTime? fixedStartTimestampOfPreviousResults,
			int cumulativeIterationNumberSum,
			out FluidDynamicSystemStateNegativeLoadsCorrected correctedState)
		{
			var previousMutationResults = fluidDynamicSystemStateMathematicallySolved.MutationResult;

			var relativeUnknownsDimensionalWrapper = this.makeUnknownsDimensional (
				                                         previousMutationResults);

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
					new ObjectWithSubstitutionInSameType<GasNodeAbstract> {
						Original = originalNode,
						Substituted = substitutedNode
					});

				List<ObjectWithSubstitutionInSameType<GasEdgeAbstract>> currentEdgeSubstitutions;

				GasNetwork networkWithFixedNodesWithLoadGadget = 
					previousMutationResults.StartingUnsolvedState.
						OriginalNetwork.makeFromRemapping (
						nodesSubstitutions,
						out currentEdgeSubstitutions);

				currentEdgeSubstitutions.ForEachFindInDo (
					lookingAtList: edgesSubstitutions,
					predicate: pair => pair.Item2.Substituted.Equals (pair.Item1.Original),
					ifFoundDo: matchingPair => matchingPair.Item2.Substituted = matchingPair.Item1.Substituted,
					ifNotFoundDo: aCurrentEdgeSubstitution => edgesSubstitutions.Add (aCurrentEdgeSubstitution));

				var initializerTransition = previousMutationResults.
					StartingUnsolvedState.InitializedBy.clone () as FluidDynamicSystemStateTransitionInitialization;
				initializerTransition.Network = networkWithFixedNodesWithLoadGadget;

				var newtonRaphsonSolverTransition = fluidDynamicSystemStateMathematicallySolved.SolvedBy;

				var newBareSystemState = new FluidDynamicSystemStateBare ();
				var newUnsolvedSystemState = newBareSystemState.doStateTransition (
					                             initializerTransition);
				var newMathematicallySolvedSystemState = newUnsolvedSystemState.doStateTransition (
					                                         newtonRaphsonSolverTransition);

				var newMathematicallySolvedSystemStateTypeSpecialized = newMathematicallySolvedSystemState 
					as FluidDynamicSystemStateMathematicallySolved;

				recursion (newMathematicallySolvedSystemStateTypeSpecialized,
					nodesSubstitutions,
					edgesSubstitutions,
					fixedStartTimestampOfPreviousResults,
					cumulativeIterationNumberSum + newMathematicallySolvedSystemStateTypeSpecialized.MutationResult.IterationNumber,
					out correctedState);
			} else {

				// modifying directly this object we put thing all together, if we wish to take
				// these informations apart, we can add properties to the final state of this transition.
				fluidDynamicSystemStateMathematicallySolved.MutationResult.ComputationStartTimestamp = 
					fixedStartTimestampOfPreviousResults;
				fluidDynamicSystemStateMathematicallySolved.MutationResult.IterationNumber = 
					cumulativeIterationNumberSum;

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
			OneStepMutationResults mutationResult)
		{
			return mutationResult.makeUnknownsDimensional ();
		}
	}
}

