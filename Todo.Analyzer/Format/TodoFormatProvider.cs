// <copyright file="TodoFormatProvider.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Todo.Analyzer.Format;

/// <summary>
/// Provide the expected <see cref="TodoFormat"/>.
/// </summary>
internal static class TodoFormatProvider
{
    /// <summary>
    /// Gets the <see cref="TodoFormat"/> of the comment from the settings.
    /// </summary>
    /// <returns>The <see cref="TodoFormat"/> of the comment from the settings.</returns>
    internal static TodoFormat GetTodoFormat()
    {
        return new GitHubTodoFormat();
    }
}