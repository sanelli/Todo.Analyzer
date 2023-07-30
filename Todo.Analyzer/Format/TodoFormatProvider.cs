// <copyright file="TodoFormatProvider.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Todo.Analyzer.Format;

/// <summary>
/// Provide the expected <see cref="TodoFormat"/>.
/// </summary>
internal static class TodoFormatProvider
{
    private const string FormatOptionKey = "todo_analyzer.comment.format";
    private const string CustomTokenRegexKey = "todo_analyzer.comment.format.custom.token_regex";
    private const string CustomValidationRegexKey = "todo_analyzer.comment.format.custom.regex";

    /// <summary>
    /// Gets the <see cref="TodoFormat"/> of the comment from the settings.
    /// </summary>
    /// <param name="analyzerConfigOptions">The analyzer configuration options.</param>
    /// <returns>The <see cref="TodoFormat"/> of the comment from the settings.</returns>
    internal static TodoFormat GetTodoFormat(AnalyzerConfigOptions analyzerConfigOptions)
    {
        return GetTodoFormatType(analyzerConfigOptions) switch
        {
            TodoFormatType.GitHub => new GitHubTodoFormat(),
            TodoFormatType.Jira => new JiraTodoFormat(),
            TodoFormatType.Custom => new CustomTodoFormat(GetCustomTokenRegex(analyzerConfigOptions), GetCustomValidationRegex(analyzerConfigOptions)),

            // Fallback to GitHub format
            _ => new GitHubTodoFormat(),
        };
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

    private static Regex GetCustomTokenRegex(AnalyzerConfigOptions analyzerConfigOptions)
    {
        try
        {
            if (analyzerConfigOptions.TryGetValue(CustomTokenRegexKey, out var customTokenRegexValue))
            {
                return new(customTokenRegexValue, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }
 #pragma warning disable CA1031
        catch (Exception)
 #pragma warning restore CA1031
        {
            // In case of failure we go back at using the default token.
        }

        return new(@"todo\s+|\s+todo|\s+todo\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    private static Regex GetCustomValidationRegex(AnalyzerConfigOptions analyzerConfigOptions)
    {
        try
        {
            if (analyzerConfigOptions.TryGetValue(CustomValidationRegexKey, out var customRegexValue))
            {
                return new(customRegexValue, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }
 #pragma warning disable CA1031
        catch (Exception)
 #pragma warning restore CA1031
        {
            // In case of failure we go back at using the default regex.
        }

        return new(@"^ TODO .*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}