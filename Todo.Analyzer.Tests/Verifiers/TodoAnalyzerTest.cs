// <copyright file="TodoAnalyzerTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Todo.Analyzer.Tests.Verifiers;

/// <summary>
/// Verifier for the analyzer.
/// </summary>
/// <typeparam name="TAnalyzer">The analyzer under test.</typeparam>
public class TodoAnalyzerTest<TAnalyzer>
    : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoAnalyzerTest{TAnalyzer}"/> class.
    /// </summary>
    /// <param name="testCode">The code that should be tested.</param>
    public TodoAnalyzerTest(string testCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(testCode);

        this.SolutionTransforms.Add((solution, projectId) =>
        {
            ArgumentNullException.ThrowIfNull(solution);

            var compilationOptions = solution.GetProject(projectId)?.CompilationOptions!;
            solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

            return solution;
        });

        this.TestCode = testCode;
    }
}