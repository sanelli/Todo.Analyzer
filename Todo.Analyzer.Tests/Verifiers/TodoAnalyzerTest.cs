// <copyright file="TodoAnalyzerTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;

using Todo.Analyzer.Tests.Helpers;

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
    /// <param name="editorconfig">The content of the .editorconfig file.</param>
    public TodoAnalyzerTest(string testCode, string? editorconfig = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(testCode);

        this.SolutionTransforms.Add((solution, projectId) =>
        {
            #pragma warning disable
            ArgumentNullException.ThrowIfNull(solution);

            var project = solution.GetProject(projectId)!;
            var compilationOptions = project.CompilationOptions!;
            compilationOptions = compilationOptions
                .WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpAnalyzerHelper.NullableWarnings))
                .WithOutputKind(OutputKind.ConsoleApplication);
            solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

            if (!string.IsNullOrWhiteSpace(editorconfig))
            {
                solution = solution.AddAnalyzerConfigDocument(DocumentId.CreateNewId(projectId), "/.editorconfig", SourceText.From(editorconfig), filePath: "/.editorconfig");
            }

            return solution;
        });

        this.TestCode = testCode;
    }
}