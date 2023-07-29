// <copyright file="TodoCommentDoNotMatchingCriteriaTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Microsoft.CodeAnalysis.Testing;

using TodoCommentDoNotMatchingCriteriaAnalyzerTest = Todo.Analyzer.Tests.Verifiers.TodoAnalyzerTest<Todo.Analyzer.TodoCommentDoNotMatchingCriteria>;

namespace Todo.Analyzer.Tests;

/// <summary>
/// Test class for <see cref="TodoCommentDoNotMatchingCriteria"/>.
/// </summary>
[UnitTest("TD0001")]
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
    /// Single line empty comment will not report any diagnostic.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineEmptyCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        //
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Single line comment with token a substring of a word it won't report a diagnostic.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineCommentWithTokenSubstringOfAWordWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // This won't fail because: the-todo-is-not-a-token-on-its-own
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
    /// Can check the comment token regardless of the casing.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CanCatchGithubTodoRegardlessOfCasing()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        // todo This will fail
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

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the first line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MultiLineCommentWithoutTodoTokenWillReportNoDiagnostics()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* Bla bla
         * Bla bla
         * Even more bla */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the first line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineCommentOnTheFirstLineWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* TODO [#123] This will not fail.
         * Bla bla
         */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the middle line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineCommentOnTheMiddleLineWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* Bla bla
         * TODO [#123] This will not fail.
         */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the middle line
    /// without the star at the beginning of the line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineCommentOnTheMiddleLineWithoutStarWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* Bla bla
         TODO [#123] This will not fail.
         */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the last line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineCommentOnTheLastLineWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /*
         * Bla bla
         * TODO [#123] This will not fail. */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the last line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineCommentOnTheLastLineWithoutStarWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /*
         Bla bla
         TODO [#123] This will not fail. */
        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line comment
    /// report no diagnostics when the token is on the first line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineCommentOnTheFirstLineWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* TODO This will fail
         * Bla bla
         */
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed github multi line comment
    /// report no diagnostics when the token is on the middle line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineCommentOnTheMiddleLineWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* Bla bla
         * TODO This will fail
         */
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed github multi line comment
    /// report no diagnostics when the token is on the middle line
    /// without the star at the beginning of the line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineCommentOnTheMiddleLineWithoutStarWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /* Bla bla
         TODO This will fail
         */
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed github multi line comment
    /// report no diagnostics when the token is on the last line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineCommentOnTheLastLineWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /*
         * Bla bla
         * TODO This will fail */
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed github multi line comment
    /// report no diagnostics when the token is on the last line.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineCommentOnTheLastLineWithoutStarWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /*
         Bla bla
         TODO This will fail */
        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Single line comment with no token in the documentation comment will be ignored
    /// and no diagnostic will be reported.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineDocumentationCommentWithoutTodoTokenWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /// <summary>
        /// This comment will be ignored because there is no T O D O token!
        /// </summary>
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Empty Single line comment will be ignored
    /// and no diagnostic will be reported.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task EmptySingleLineDocumentationCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /// <summary>
        ///
        /// </summary>
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted single line documentation comment
    /// report no diagnostics.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoSingleLineDocumentationCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /// <summary>
        /// TODO [#123] This will succeed.
        /// </summary>
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line documentation comment on top of a method
    /// generates a warning message.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoSingleLineDocumentationCommentWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /// <summary>
        /// TODO This will fail
        /// </summary>
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 4));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Multi line comment with no token in the documentation comment will be ignored
    /// and no diagnostic will be reported.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MultiLineDocumentationCommentWithoutTodoTokenWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /** <summary>
         * This comment will be ignored because there is no T O D O token!
         * </summary>
         */
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Empty Multi line comment will be ignored
    /// and no diagnostic will be reported.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task EmptyMultiLineDocumentationCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /** <summary>
         *
         * </summary>
         */
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly github formatted multi line documentation comment
    /// report no diagnostics.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoMultiLineDocumentationCommentWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /** <summary>
         * TODO [#123] This will succeed.
         * </summary>
         */
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed multi line documentation comment on top of a method
    /// generates a warning message.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoMultiLineDocumentationCommentWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest("""
        /** <summary>
         * TODO this will fail
         * </summary>
         */
        int MyMethod() { return -1; }

        System.Console.WriteLine("Hello world!");
        """);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 4));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}