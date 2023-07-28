// <copyright file="TodoFormatType.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Todo.Analyzer.Format;

/// <summary>
/// The format of the expected comment.
/// </summary>
internal enum TodoFormatType
{
    /// <summary>
    /// No format specified.
    /// </summary>
    None,

    /// <summary>
    /// The github format.
    /// </summary>
    GitHub,

    /// <summary>
    /// The Jir format.
    /// </summary>
    Jira,

    /// <summary>
    /// A format defined by the developer.
    /// </summary>
    Custom,
}