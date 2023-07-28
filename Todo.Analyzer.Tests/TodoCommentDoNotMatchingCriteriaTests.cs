// <copyright file="TodoCommentDoNotMatchingCriteriaTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Microsoft.CodeAnalysis.Testing;

using TodoCommentDoNotMatchingCriteriaAnalyzerTest = Todo.Analyzer.Tests.Verifiers.TodoAnalyzerTest<Todo.Analyzer.TodoCommentDoNotMatchingCriteria>;

namespace Todo.Analyzer.Tests;

/// <summary>
/// Test class for <see cref="TodoCommentDoNotMatchingCriteria"/>.
/// </summary>
public sealed class TodoCommentDoNotMatchingCriteriaTests
{
    /// <summary>
    /// Single line comment with no token in the comment will be ignored
    /// and no diagnostic will be reported.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineCommentWithoutTodoTokenWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // This comment will be ignored because there is no T O D O token!
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted single line comment
    /// report no diagnostics.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoSingleLineCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // TODO [#123] This will succeed.
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment on top of a method
    /// generates a warning message.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoSingleLineCommentWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // TODO This will fail
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment on top of a method
    /// generates a warning message.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoSingleLineCommentWithMissingTrailingStopWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // TODO [#123] This will fail
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}