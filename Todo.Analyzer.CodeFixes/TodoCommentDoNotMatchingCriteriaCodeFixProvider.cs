// <copyright file="TodoCommentDoNotMatchingCriteriaCodeFixProvider.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Todo.Analyzer.CodeFixes;

/// <summary>
/// Code fix provider for <see cref="TodoCommentDoNotMatchingCriteria"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TodoCommentDoNotMatchingCriteriaCodeFixProvider))]
[Shared]
public sealed class TodoCommentDoNotMatchingCriteriaCodeFixProvider
    : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray<string>.Empty;

    /// <inheritdoc />
    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Implement me
        // See: https://github.com/dotnet/samples/blob/main/csharp/roslyn-sdk/Tutorials/MakeConst/MakeConst.CodeFixes/MakeConstCodeFixProvider.cs
        return Task.CompletedTask;
    }
}