// <copyright file="TodoFormatProvider.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.Diagnostics;

namespace Todo.Analyzer.Format;

/// <summary>
/// Provide the expected <see cref="TodoFormat"/>.
/// </summary>
internal static class TodoFormatProvider
{
    private const string FormatOptionKey = "todo_analyzer.comment.format";

    /// <summary>
    /// Gets the <see cref="TodoFormat"/> of the comment from the settings.
    /// </summary>
    /// <param name="analyzerConfigOptions">The analyzer configuration options.</param>
    /// <returns>The <see cref="TodoFormat"/> of the comment from the settings.</returns>
    internal static TodoFormat GetTodoFormat(AnalyzerConfigOptions analyzerConfigOptions)
    {
        switch (GetTodoFormatType(analyzerConfigOptions))
        {
         case TodoFormatType.GitHub:
             return new GitHubTodoFormat();
         case TodoFormatType.Jira:
             return new JiraTodoFormat();
        }

        // Fallback to GitHub
        return new GitHubTodoFormat();
    }

    private static TodoFormatType GetTodoFormatType(AnalyzerConfigOptions analyzerConfigOptions)
    {
        if (analyzerConfigOptions.TryGetValue(FormatOptionKey, out var formatOptionValue)
            && Enum.TryParse(formatOptionValue, true, out TodoFormatType formatOption))
        {
            return formatOption;
        }

        return TodoFormatType.None;
    }
}