// <copyright file="GitHubTodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// GitHub comment style implementation of <see cref="TodoFormat"/>.
/// </summary>
internal sealed class GitHubTodoFormat
    : TodoFormat
{
    private static readonly Regex ExpectedToMatch = new(@"^ TODO \[\#(?<taskId>[0-9]+)\] .*(\.|\!|\?)$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubTodoFormat"/> class.
    /// </summary>
    public GitHubTodoFormat()
        : base(DefaultTodoMatchRegex, ExpectedToMatch)
    {
    }
}