// <copyright file="CustomTodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// Github comment style implementation of <see cref="TodoFormat"/>.
/// </summary>
internal sealed class CustomTodoFormat
    : TodoFormat
{
    private readonly Regex tokenRegex;
    private readonly Regex validationRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTodoFormat"/> class.
    /// </summary>
    /// <param name="tokenRegex">The regular expression used to identify the token.</param>
    /// <param name="validationRegex">The regular expression used to identify if the comment match the criteria.</param>
    public CustomTodoFormat(Regex tokenRegex, Regex validationRegex)
    {
        this.tokenRegex = tokenRegex;
        this.validationRegex = validationRegex;
    }

    /// <inheritdoc/>
    internal override bool IsTodoCommentLine(string commentLine)
        => this.tokenRegex.Match(commentLine).Success;

    /// <inheritdoc/>
    internal override bool HasValidCommentLine(string commentLine)
        => this.validationRegex.Match(commentLine).Success;
}