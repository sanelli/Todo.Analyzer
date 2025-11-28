// <copyright file="TodoFormat.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Todo.Analyzer.Format;

/// <summary>
/// The format of the comment.
/// </summary>
internal abstract class TodoFormat
{
    /// <summary>
    /// The regex expression matching the default token.
    /// </summary>
    protected internal static readonly Regex DefaultTodoMatchRegex = new(@"\btodo\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// The regular expression group name to extract the task identifier.
    /// </summary>
    private const string TaskIdGroupName = "taskId";

    private readonly Regex tokenRegex;
    private readonly Regex validationRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoFormat"/> class.
    /// </summary>
    /// <param name="tokenRegex">The regular expression used to identify the token.</param>
    /// <param name="validationRegex">The regular expression used to identify if the comment match the criteria.</param>
    protected TodoFormat(Regex tokenRegex, Regex validationRegex)
    {
        this.tokenRegex = tokenRegex;
        this.validationRegex = validationRegex;
    }

    /// <summary>
    /// Check if the command line should be validated.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <returns><c>true</c> if the comment line should be validated.</returns>
    internal bool IsTodoCommentLine(string commentLine)
        => !string.IsNullOrWhiteSpace(commentLine) && this.tokenRegex.Match(commentLine).Success;

    /// <summary>
    /// Check if the comment line matched the criteria.
    /// </summary>
    /// <param name="commentLine">A single line of comment without comment markers.</param>
    /// <param name="taskId">The (optional) task identifier.</param>
    /// <returns><c>true</c> if the comment line has a valid format.</returns>
    internal bool HasValidCommentLine(string commentLine, out string? taskId)
    {
        var match = this.validationRegex.Match(commentLine);
        if (match.Success)
        {
            var groupCollection = match.Groups[TaskIdGroupName];
            taskId = groupCollection.Success ? groupCollection.Value : null;
            return true;
        }

        taskId = null;
        return false;
    }
}