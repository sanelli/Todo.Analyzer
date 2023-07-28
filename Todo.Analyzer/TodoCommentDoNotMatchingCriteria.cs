// <copyright file="TodoCommentDoNotMatchingCriteria.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

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
                    break;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    break;
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    break;
            }
        }
    }

    private static void ReportDiagnosticIfCommentLineDoesNotMatchCriteria(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat, string comment)
    {
        if (todoFormat.IsTodoCommentLine(comment) && !todoFormat.HasValidCommentLine(comment))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, syntaxNode.GetLocation()));
        }
    }

    private static void HandleSingleLineCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia syntaxNode, TodoFormat todoFormat)
    {
        var comment = syntaxNode.ToFullString().Substring(2);
        ReportDiagnosticIfCommentLineDoesNotMatchCriteria(context, syntaxNode, todoFormat, comment);
    }
}