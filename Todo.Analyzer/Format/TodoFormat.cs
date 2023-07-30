// <copyright file="TodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Todo.Analyzer.Format;

/// <summary>
/// The format of the comment.
/// </summary>
internal abstract class TodoFormat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoFormat"/> class.
    /// </summary>
    protected TodoFormat()
    {
    }

    /// <summary>
    /// Check if the command line should be validated.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <returns><c>true</c> if the comment line should be validated.</returns>
    internal virtual bool IsTodoCommentLine(string commentLine)
    {
        return !string.IsNullOrWhiteSpace(commentLine) && Array.Exists(commentLine.Split(), token => "todo".Equals(token, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Check if the comment line matched the criteria.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <returns><c>true</c> if the comment line has a valid format.</returns>
    internal abstract bool HasValidCommentLine(string commentLine);
}