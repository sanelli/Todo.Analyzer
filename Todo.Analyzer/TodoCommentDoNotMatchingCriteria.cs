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
/// Analyzer reporting the TA0001 warning.
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

    private static readonly Regex StartsWithSpacesAndSlashAndStarAndStar = new(@"^\s*\/\*\*", RegexOptions.Compiled);

    private static readonly Regex StartsWithSpacesAndStar = new(@"^\s*\*", RegexOptions.Compiled);

    private static readonly Regex EndsWithSpaceAndStarAndSlashAndSpaces = new(@"\s\*\/\s*$", RegexOptions.Compiled);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
 #pragma warning disable CA1062
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
 #pragma warning restore CA1062
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationStart);
    }

    private static void RegisterCompilationStart(CompilationStartAnalysisContext startContext)
    {
        var optionsProvider = startContext.Options.AnalyzerConfigOptionsProvider;
        startContext.RegisterSyntaxTreeAction(actionContext => AnalyzeSyntaxTree(actionContext, optionsProvider));
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
    {
        var options = analyzerConfigOptionsProvider.GetOptions(context.Tree);
        var todoFormat = TodoFormatProvider.GetTodoFormat(options);

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
                    HandleSingleLineDocumentationCommentTrivia(context, commentNode, todoFormat);
                    break;
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    HandleMultiLineDocumentationCommentTrivia(context, commentNode, todoFormat);
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
        var commentLine = syntaxNode.ToFullString().TrimStart();

        // Remove the //
        commentLine = commentLine.Substring(2);
        if (string.IsNullOrWhiteSpace(commentLine))
        {
            return;
        }

        ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, commentLine, todoFormat, syntaxNode.GetLocation());
    }

    private static void HandleMultiLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var commentLines = syntaxNode.ToFullString().Split('\n').ToArray();
        foreach (var commentLine in commentLines)
        {
            var cleanCommentLine = commentLine.Replace("\r", string.Empty);

            if (string.IsNullOrWhiteSpace(cleanCommentLine))
            {
                continue;
            }

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

    private static void HandleSingleLineDocumentationCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var commentLines = syntaxNode.ToFullString().Split('\n').ToArray();
        foreach (var commentLine in commentLines)
        {
            var cleanCommentLine = commentLine.Replace("\r", string.Empty).TrimStart();
            if (string.IsNullOrWhiteSpace(cleanCommentLine))
            {
                continue;
            }

            // Remove the "///"
            cleanCommentLine = cleanCommentLine.Substring(3);
            ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, cleanCommentLine, todoFormat, syntaxNode.GetLocation());
        }
    }

    private static void HandleMultiLineDocumentationCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var commentLines = syntaxNode.ToFullString().Split('\n').ToArray();
        foreach (var commentLine in commentLines)
        {
            var cleanCommentLine = commentLine.Replace("\r", string.Empty);

            if (string.IsNullOrWhiteSpace(cleanCommentLine))
            {
                continue;
            }

            // Remove trailing */
            // Need to remove trailing first in order to avoid clash with leading *.
            if (EndsWithSpaceAndStarAndSlashAndSpaces.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimEnd().Substring(0, cleanCommentLine.Length - 3);
            }

            // Remove initial /** or *
            if (StartsWithSpacesAndSlashAndStarAndStar.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimStart().Substring(3);
            }
            else if (StartsWithSpacesAndStar.Match(cleanCommentLine).Success)
            {
                cleanCommentLine = cleanCommentLine.TrimStart().Substring(1);
            }

            ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, cleanCommentLine, todoFormat, syntaxNode.GetLocation());
        }
    }
}