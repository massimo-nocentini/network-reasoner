using System;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Globalization;
using MathNet.Numerics.LinearAlgebra.Double.Solvers.StopCriterium;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Double.Solvers.Iterative;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.math.algebra.tests
{
	[TestFixture()]
	public class MathDotNetLearningTest
	{
		[Test()]
		public void SimpleMatrixDefinition ()
		{
			// 1. Initialize a new instance of the matrix from a 2D array. This constructor will allocate a completely new memory block for storing the dense matrix.
            var matrix1 = DenseMatrix.OfArray(new[,] { { 1.0, 2.0, 3.0 }, { 4.0, 5.0, 6.0 } });

            // 2. Initialize a new instance of the empty square matrix with a given order.
            var matrix2 = new DenseMatrix(3);

            // 3. Initialize a new instance of the empty matrix with a given size.
            var matrix3 = new DenseMatrix(2, 3);

            // 4. Initialize a new instance of the matrix with all entries set to a particular value.
            var matrix4 = DenseMatrix.Create(2, 3, (i, j) => 3.0);

            // 4. Initialize a new instance of the matrix from a one dimensional array. This array should store the matrix in column-major order.
            var matrix5 = new DenseMatrix(2, 3, new[] { 1.0, 4.0, 2.0, 5.0, 3.0, 6.0 });

            // 5. Initialize a square matrix with all zero's except for ones on the diagonal. Identity matrix (http://en.wikipedia.org/wiki/Identity_matrix).
            var matrixI = DenseMatrix.Identity(5);

            // Format matrix output to console
            var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            formatProvider.TextInfo.ListSeparator = " ";

            Console.WriteLine(@"Matrix 1");
            Console.WriteLine(matrix1.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            Console.WriteLine(@"Matrix 2");
            Console.WriteLine(matrix2.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            Console.WriteLine(@"Matrix 3");
            Console.WriteLine(matrix3.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            Console.WriteLine(@"Matrix 4");
            Console.WriteLine(matrix4.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            Console.WriteLine(@"Matrix 5");
            Console.WriteLine(matrix5.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            Console.WriteLine(@"Identity matrix");
            Console.WriteLine(matrixI.ToString("#0.00\t", formatProvider));
            Console.WriteLine();
		}

		[Test()]
		public void attempt_system_resolution(){
			// Format matrix output to console
            var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            formatProvider.TextInfo.ListSeparator = " ";

            // Solve next system of linear equations (Ax=b):
            // 5*x + 2*y - 4*z = -7
            // 3*x - 7*y + 6*z = 38
            // 4*x + 1*y + 5*z = 43

            // Create matrix "A" with coefficients
            var matrixA = DenseMatrix.OfArray(new[,] { { 5.00, 2.00, -4.00 }, { 3.00, -7.00, 6.00 }, { 4.00, 1.00, 5.00 } });
            Console.WriteLine(@"Matrix 'A' with coefficients");
            Console.WriteLine(matrixA.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // Create vector "b" with the constant terms.
            var vectorB = new DenseVector(new[] { -7.0, 38.0, 43.0 });
            Console.WriteLine(@"Vector 'b' with the constant terms");
            Console.WriteLine(vectorB.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // Create stop criteriums to monitor an iterative calculation. There are next available stop criteriums:
            // - DivergenceStopCriterium: monitors an iterative calculation for signs of divergence;
            // - FailureStopCriterium: monitors residuals for NaN's;
            // - IterationCountStopCriterium: monitors the numbers of iteration steps;
            // - ResidualStopCriterium: monitors residuals if calculation is considered converged;

            // Stop calculation if 1000 iterations reached during calculation
            var iterationCountStopCriterium = new IterationCountStopCriterium(1000);

            // Stop calculation if residuals are below 1E-10 --> the calculation is considered converged
            var residualStopCriterium = new ResidualStopCriterium(1e-10);
 
            // Create monitor with defined stop criteriums
            var monitor = new Iterator(new IIterationStopCriterium[] { iterationCountStopCriterium, residualStopCriterium });

            // Create Bi-Conjugate Gradient Stabilized solver
            var solver = new BiCgStab(monitor);

            // 1. Solve the matrix equation
            var resultX = solver.Solve(matrixA, vectorB);
            Console.WriteLine(@"1. Solve the matrix equation");
            Console.WriteLine();

            // 2. Check solver status of the iterations.
            // Solver has property IterationResult which contains the status of the iteration once the calculation is finished.
            // Possible values are:
            // - CalculationCancelled: calculation was cancelled by the user;
            // - CalculationConverged: calculation has converged to the desired convergence levels;
            // - CalculationDiverged: calculation diverged;
            // - CalculationFailure: calculation has failed for some reason;
            // - CalculationIndetermined: calculation is indetermined, not started or stopped;
            // - CalculationRunning: calculation is running and no results are yet known;
            // - CalculationStoppedWithoutConvergence: calculation has been stopped due to reaching the stopping limits, but that convergence was not achieved;
            Console.WriteLine(@"2. Solver status of the iterations");
            Console.WriteLine(solver.IterationResult);
            Console.WriteLine();

            // 3. Solution result vector of the matrix equation
            Console.WriteLine(@"3. Solution result vector of the matrix equation");
            Console.WriteLine(resultX.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // 4. Verify result. Multiply coefficient matrix "A" by result vector "x"
            var reconstructVecorB = matrixA * resultX;
            Console.WriteLine(@"4. Multiply coefficient matrix 'A' by result vector 'x'");
            Console.WriteLine(reconstructVecorB.ToString("#0.00\t", formatProvider));
            Console.WriteLine();
		}

		[Test()]
		public void CheckIfEqualsIsOverridden(){

			var matrixA = DenseMatrix.OfArray(new[,] { { 5.00, 2.00, -4.00 }, { 3.00, -7.00, 6.00 }, { 4.00, 1.00, 5.00 } });

			var matrixC = DenseMatrix.OfArray(new[,] { { 5.00, 2.00, -4.00 }, { 3.00, -7.00, 6.00 }, { 4.00, 1.00, 5.00 } });
			
            var matrixB = new DenseMatrix(2, 3, new[] { 1.0, 4.0, 2.0, 5.0, 3.0, 6.0 });

			var matrixD = DenseMatrix.OfArray(new[,] { { 65.00, 2.01, -4.54 }, { 33.00, -7.63, 56.00 }, { 4.01, 12.00, 54 } });            

			Assert.AreEqual(matrixA, matrixC);
			Assert.AreNotEqual(matrixA, matrixB);
			Assert.AreNotEqual(matrixA, matrixD);
		}

		[Test()]
		public void creating_a_sparse_matrix_from_an_indexed_collection(){

			List<Tuple<int, int, double>> indices = 
				new List<Tuple<int, int, double>>();

			var firstRowThirdColumn = 4.3;
			var secondRowFirstColumn = 1.2;
			var thirdRowSecondColumn = 47.93;

			indices.Add(new Tuple<int, int, double>(0,2,firstRowThirdColumn));
			indices.Add(new Tuple<int, int, double>(1,0,secondRowFirstColumn));
			indices.Add(new Tuple<int, int, double>(2,1,thirdRowSecondColumn));
			

			var aMatrix = SparseMatrix.OfIndexed(3,3,indices);

			var expectedMatrix = DenseMatrix.OfArray(new[,] { 
				{ 0.00, 0.00, firstRowThirdColumn }, 
				{ secondRowFirstColumn, 0.00, 0.00 }, 
				{ 0.00, thirdRowSecondColumn, 0.00 } });
			
			Assert.AreEqual(expectedMatrix, aMatrix);
		}
	}
}

