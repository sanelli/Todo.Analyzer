// <copyright file="PlainTodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// Plain comment style implementation of <see cref="TodoFormat"/>.
/// </summary>
internal sealed class PlainTodoFormat
    : TodoFormat
{
    private static readonly Regex ExpectedToMatch = new(@"^ TODO\: .*(\.|\!|\?)$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="PlainTodoFormat"/> class.
    /// </summary>
    public PlainTodoFormat()
        : base(DefaultTodoMatchRegex, ExpectedToMatch)
    {
    }
}