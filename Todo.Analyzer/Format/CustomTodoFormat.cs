// <copyright file="CustomTodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// Custom comment style implementation of <see cref="TodoFormat"/>.
/// </summary>
internal sealed class CustomTodoFormat
    : TodoFormat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTodoFormat"/> class.
    /// </summary>
    /// <param name="tokenRegex">The regular expression used to identify the token.</param>
    /// <param name="validationRegex">The regular expression used to identify if the comment match the criteria.</param>
    public CustomTodoFormat(Regex tokenRegex, Regex validationRegex)
        : base(tokenRegex, validationRegex)
    {
    }
}