// <copyright file="TodoCommentDoNotMatchingCriteria.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Todo.Analyzer;

/// <summary>
/// Analyzer reporting the TD0001 warning.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TodoCommentDoNotMatchingCriteria
    : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic identifier of this rule.
    /// </summary>
    public const string DiagnosticId = "TA0001";

    private const string Category = "Documentation";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.TD0001_Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.TD0001_MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.TD0001_Description), Resources.ResourceManager, typeof(Resources));
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

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
        context.RegisterSyntaxNodeAction(this.AnalyzeSingleLineCommentTrivia, SyntaxKind.SingleLineCommentTrivia);
    }

    private void AnalyzeSingleLineCommentTrivia(SyntaxNodeAnalysisContext context)
    {
        // Implement me.
    }
}