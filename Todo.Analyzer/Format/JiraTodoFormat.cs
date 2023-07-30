// <copyright file="JiraTodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// Github comment style implementation of <see cref="TodoFormat"/>.
/// </summary>
internal sealed class JiraTodoFormat
    : TodoFormat
{
    private static readonly Regex ExpectedToMatch = new(@"^ TODO \[[a-zA-Z0-9]+\-[0-9]+\] .*\.$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="JiraTodoFormat"/> class.
    /// </summary>
    public JiraTodoFormat()
        : base()
    {
    }

    /// <inheritdoc/>
    internal override bool HasValidCommentLine(string commentLine)
        => ExpectedToMatch.Match(commentLine).Success;
}