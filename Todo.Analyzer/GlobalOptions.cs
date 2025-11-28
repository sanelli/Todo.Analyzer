// <copyright file="GlobalOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis.Diagnostics;

namespace Todo.Analyzer;

/// <summary>
/// Load global options from the analyzer configuration.
/// </summary>
internal sealed class GlobalOptions
{
    private const string AlwaysReportKey = "todo_analyzer.always_report";
    private const string ReportTasksKey = "todo_analyzer.report_tasks";

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalOptions"/> class.
    /// </summary>
    /// <param name="analyzerConfigOptions">The analyzer options.</param>
    public GlobalOptions(AnalyzerConfigOptions analyzerConfigOptions)
    {
        this.AlwaysReport = analyzerConfigOptions.TryGetValue(AlwaysReportKey, out var alwaysReportValue)
            && IsTruthy(alwaysReportValue);
        this.ReportTasks = analyzerConfigOptions.TryGetValue(ReportTasksKey, out var reportTasksValues)
            ? ToArray(reportTasksValues)
            : [];
    }

#pragma warning disable S1135
    /// <summary>
    /// Gets a value indicating whether to report a warning if a TODO is present.
    /// </summary>
#pragma warning restore S1135
    public bool AlwaysReport { get; }

    /// <summary>
    /// Gets a list of tasks item to report even if the comment respect the criteria.
    /// </summary>
    public string[] ReportTasks { get; }

    private static bool IsTruthy(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var truthy = new[] { "true", "yes", "1", "enable" };
        return truthy.Any(truth => truth.Equals(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string[] ToArray(string? values)
    {
        if (string.IsNullOrWhiteSpace(values))
        {
            return [];
        }

        return values!.Split(',').Select(value => value.Trim()).ToArray();
    }
}