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
    /// <param name="todoFormatType">The expected type of the comment.</param>
    protected TodoFormat(TodoFormatType todoFormatType)
    {
        this.TodoFormatType = todoFormatType;
    }

    /// <summary>
    /// Gets the format of the comment.
    /// </summary>
    internal TodoFormatType TodoFormatType { get; }

    /// <summary>
    /// Check if the command line should be validated.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <returns><c>true</c> if the comment line should be validated.</returns>
    internal virtual bool IsTodoCommentLine(string commentLine) => !string.IsNullOrWhiteSpace(commentLine) && commentLine.IndexOf("todo", StringComparison.InvariantCultureIgnoreCase) >= 0;

    /// <summary>
    /// Check if the comment line matched the criteria.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <returns><c>true</c> if the comment line has a valid format.</returns>
    internal abstract bool HasValidCommentLine(string commentLine);
}