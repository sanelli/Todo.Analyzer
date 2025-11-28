# Comment.Todo.Analyzer
.NET analyzer for C# making sure that all TODO comments respect the same coding standards.

## Options
These option can be setup inside the `.editorconfig`:

- `todo_analyzer.comment.format`
  - Description: The format of the comment containing the TODO. 
  - Accepted values: `github`, `jira`, `custom`, `plain`
  - Default: `github`
  - Example: `todo_analyzer.comment.format = github`
- `todo_analyzer.comment.format.custom.token_regex`
    - Description: The token used to identify if the comment is a TODO when `todo_analyzer.comment.format` is `custom`.
    - Default: `\bTODO\b`
    - Example: `todo_analyzer.comment.format.custom.token = やるべきこと`
    - Note: This is a regular expression.
- `todo_analyzer.comment.format.custom.regex`
    - Description: The regular expression used to check if the comment match the expected criteria when `todo_analyzer.comment.format` is `custom`.
    - Default: `^ TODO .*$`
    - Example: `todo_analyzer.comment.format.custom.regex = ^ TODO \[MY-PROJECT\] \[TASK-(?<taskId>\d+)\] .+\.$`
- `todo_analyzer.always_report`
    - Description: Enable error TA0002 to be reported when any TODO comment is identified.
    - Default: `false`
    - Example: `todo_analyzer.always_report = true`
- `todo_analyzer.always_report`
    - Description: Enable error TA0003 to report TODO comments containing specified task identifier.
    - Default: `<empty>`
    - Example: `todo_analyzer.report_tasks = 33, 56, 1984`

## Default formats
- `github` (Default) : `^ TODO \[\#[0-9]+\] .*(\.|\!|\?)$`
  - Example: `TODO [#33] This needs to be handled.` 
- `jira`: `^ TODO \[[a-zA-Z0-9]+\-[0-9]+\] .*(\.|\!|\?)$`
    - Example: `TODO [COMMON-1234] This needs to be handled!`
- `plain` : `^ TODO\: .*(\.|\!|\?)$`
    - Example: `TODO: Should we handle this?`

## Rules
| Rule ID | Category      | Severity | Url                 |
|---------|---------------|----------|---------------------|
| TA0001  | Documentation | Warning  | [TA0001](TA0001.md) |
| TA0002  | Documentation | Warning  | [TA0002](TA0002.md) |
| TA0003  | Documentation | Warning  | [TA0003](TA0003.md) |