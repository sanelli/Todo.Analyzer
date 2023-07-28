// <copyright file="TodoCommentDoNotMatchingCriteria.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Todo.Analyzer.Format;

namespace Todo.Analyzer;

/// <summary>
/// Analyzer reporting the TD0001 warning.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TodoCommentDoNotMatchingCriteria
    : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Rule = new(
        "TA0001",
        new LocalizableResourceString(nameof(Resources.TD0001_Title), Resources.ResourceManager, typeof(Resources)),
        new LocalizableResourceString(nameof(Resources.TD0001_MessageFormat), Resources.ResourceManager, typeof(Resources)),
        "Documentation",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: new LocalizableResourceString(nameof(Resources.TD0001_Description), Resources.ResourceManager, typeof(Resources)));

    private static readonly Regex StartsWithSpacesAndSlashAndStar = new(@"^\s*\/\*", RegexOptions.Compiled);

    private static readonly Regex StartsWithSpacesAndStar = new(@"^\s*\*", RegexOptions.Compiled);

    private static readonly Regex EndsWithSpaceAndStarAndSlashAndSpaces = new(@"\s\*\/\s*$", RegexOptions.Compiled);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var todoFormat = TodoFormatProvider.GetTodoFormat();

        var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
        foreach (var commentNode in root.DescendantTrivia())
        {
            switch (commentNode.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                    HandleSingleLineCommentTrivia(context, commentNode, todoFormat);
                    break;
                case SyntaxKind.MultiLineCommentTrivia:
                    HandleMultiLineCommentTrivia(context, commentNode, todoFormat);
                    break;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    break;
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    break;
            }
        }
    }

    private static void ReportDiagnosticIfCommentLineDoesNotMatchCriteria(SyntaxTreeAnalysisContext context, string commentLine, TodoFormat todoFormat, Location? location)
    {
        if (todoFormat.IsTodoCommentLine(commentLine) && !todoFormat.HasValidCommentLine(commentLine))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }
    }

    private static void HandleSingleLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var commentLine = syntaxNode.ToFullString().TrimStart().Substring(2);
        ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, commentLine, todoFormat, syntaxNode.GetLocation());
    }

    private static void HandleMultiLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var commentLines = syntaxNode.ToFullString().Split('\n').ToArray();
        foreach (var commentLine in commentLines)
        {
            var cleanCommentLine = commentLine.Replace("\r", string.Empty);

            // Remove trailing */
            // Need to remove trailing first in order to avoid clash with leading *.
            if (EndsWithSpaceAndStarAndSlashAndSpaces.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimEnd().Substring(0, cleanCommentLine.Length - 3);
            }

            // Remove initial /* or *
            if (StartsWithSpacesAndSlashAndStar.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimStart().Substring(2);
            }
            else if (StartsWithSpacesAndStar.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimStart().Substring(1);
            }

            ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, cleanCommentLine, todoFormat, syntaxNode.GetLocation());
        }
    }
}