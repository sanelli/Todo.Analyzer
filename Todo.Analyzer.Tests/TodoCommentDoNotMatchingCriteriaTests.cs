// <copyright file="TodoCommentDoNotMatchingCriteriaTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

using TodoCommentDoNotMatchingCriteriaAnalyzerTest = Todo.Analyzer.Tests.Verifiers.TodoAnalyzerTest<Todo.Analyzer.TodoCommentDoNotMatchingCriteria>;

namespace Todo.Analyzer.Tests;

/// <summary>
/// Test class for <see cref="TodoCommentDoNotMatchingCriteria"/>.
/// </summary>
public sealed class TodoCommentDoNotMatchingCriteriaTests
{
    /// <summary>
    /// Test that a malformed single line comment on top of a method
    /// generates a warning message.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedTodoSingleLineCommentOnTopOfMethodReportsDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(@"""
        public class Test
        {
            // TODO: Implement me
            public int Method()
            { 
                return 0;
            }
        }
        """);
        test.ExpectedDiagnostics.Add(new DiagnosticResult(TodoCommentDoNotMatchingCriteria.DiagnosticId, DiagnosticSeverity.Warning));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}