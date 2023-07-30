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
    private const string GitHubEditorConfig = """
            root = true
            [*]
            todo_analyzer.comment.format = github
            """;

    private const string JiraEditorConfig = """
            root = true
            [*]
            todo_analyzer.comment.format = jira
            """;

    private const string MalformedEditorConfig = """
            root = true
            [*]
            todo_analyzer.comment.format = malformed
            """;

    private const string CustomFormatEditorConfig = """
            root = true
            [*]
            todo_analyzer.comment.format = custom
            todo_analyzer.comment.format.custom.token_regex = dafare
            todo_analyzer.comment.format.custom.regex = ^ DAFARE .*\.
            """;

    private const string CustomFormatEditorConfigWithoutTokenRegex = """
            root = true
            [*]
            todo_analyzer.comment.format = custom
            todo_analyzer.comment.format.custom.regex = ^ TODO FOO .*\.
            """;

    private const string CustomFormatEditorConfigWithoutRegex = """
            root = true
            [*]
            todo_analyzer.comment.format = custom
            todo_analyzer.comment.format.custom.token_regex = TODO
            """;

    private const string CustomFormatEditorConfigWithWrongSettings = """
            root = true
            [*]
            todo_analyzer.comment.format = custom
            todo_analyzer.comment.format.custom.token_regex = \q
            todo_analyzer.comment.format.custom.regex = \q
            """;

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

    /// <summary>
    /// Test that a correctly github formatted single line comment
    /// report no diagnostics. The editorconfig content is provided by the user.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedGithubTodoSingleLineCommentAndEditorconfigWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO [#123] This will succeed.
            System.Console.WriteLine("Hello world!");
            """,
            GitHubEditorConfig);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment on top of a method
    /// generates a warning message. The editorconfig content is provided by the user.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedGithubTodoSingleLineCommentAndEditorconfigWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO This will fail
            System.Console.WriteLine("Hello world!");
            """,
            GitHubEditorConfig);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correct GitHub comment will fail with a Jira setting.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task GitHubCommentWithJiraConfigurationWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO [#123] This will fail because it is not a JIRA comment.
            System.Console.WriteLine("Hello world!");
            """,
            JiraEditorConfig);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correctly JIRA formatted single line comment
    /// report no diagnostics. The editorconfig content is provided by the user.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectlyFormattedJiraTodoSingleLineCommentAndEditorconfigWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO [STEFANO-1984] This will succeed.
            System.Console.WriteLine("Hello world!");
            """,
            JiraEditorConfig);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line Jira comment on top of a method
    /// generates a warning message. The editorconfig content is provided by the user.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedJiraTodoSingleLineCommentAndEditorconfigWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO This will fail
            System.Console.WriteLine("Hello world!");
            """,
            JiraEditorConfig);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a correct GitHub comment will fail with a Jira setting.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task JiraCommentWithGithubConfigurationWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO [STEFANO-1984] This will fail because it is not a JIRA comment.
            System.Console.WriteLine("Hello world!");
            """,
            GitHubEditorConfig);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed editorconfig format will fallback on GitHub format.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedFormatInEditorConfigFallbackesToGitHub()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO [#123] This will succeed.
            System.Console.WriteLine("Hello world!");
            """,
            MalformedEditorConfig);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment with a custom format reports an error.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedSingleLineCommentWithCustomFormatWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // C'è qualcosa dafare qui.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfig);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line comment with a custom format does not report an error.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectSingleLineCommentWithCustomFormatWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // DAFARE Devi completare questo.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfig);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line comment with a custom format token does not report an error.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task CorrectSingleLineCommentWithCustomFormatWithoutCustomTokenWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // Nothing to see here.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfig);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment with a custom format
    /// and without custom token regex reports an error.
    /// This tests that a missing <c>custom.token_regex</c> settings will fallback to default.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedSingleLineCommentWithCustomFormatAndWithoutCustomTokenRegexWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // This TODO needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithoutTokenRegex);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line comment with a custom format
    /// and without custom token regex does not reports an error.
    /// This tests that a missing <c>custom.token_regex</c> settings will fallback to default.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineCommentWithCustomFormatAndWithoutCustomTokenRegexWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO FOO this needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithoutTokenRegex);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a malformed single line comment with a custom format
    /// and without custom regex reports an error.
    /// This tests that a missing <c>custom.regex</c> settings will fallback to default.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedSingleLineCommentWithCustomFormatAndWithoutCustomRegexWillReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // This TODO needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithoutRegex);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line comment with a custom format
    /// and without custom regex does not reports an error.
    /// This tests that a missing <c>custom.regex</c> settings will fallback to default.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineCommentWithCustomFormatAndWithoutCustomRegexWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO this needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithoutRegex);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line comment with a custom format
    /// and incorrect token_regex and regex fallback to defaults.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task SingleLineCommentWithCustomFormatAndIncorrectSettingsWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // TODO this needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithWrongSettings);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Test that a single line malformed comment with a custom format
    /// and incorrect token_regex and regex fallback to defaults.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    [Fact]
    public async Task MalformedSingleLineCommentWithCustomFormatAndIncorrectSettingsWillNotReportDiagnostic()
    {
        var test = new TodoCommentDoNotMatchingCriteriaAnalyzerTest(
            """
            // This TODO needs to be done.
            System.Console.WriteLine("Hello world!");
            """,
            CustomFormatEditorConfigWithWrongSettings);
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(TodoCommentDoNotMatchingCriteria.Rule.Id, TodoCommentDoNotMatchingCriteria.Rule.DefaultSeverity)
                .WithLocation(1, 1));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}