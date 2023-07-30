# Comment.Todo.Analyzer
.NET analyzer for C# making sure that all TODO comments respect the same coding standards.

## Options
These option can be setup inside the `.editorconfig`:

- `todo_analyzer.comment.format`
  - Description: The format of the comment containing the TODO. 
  - Accepted values: `github`, `jira`, `custom`
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
    - Example: `todo_analyzer.comment.format.custom.regex = ^ TODO \[MY-PROJECT\] [\d+] .+\.$`

## Default formats
- `github` (Default) : `^ TODO \[\#[0-9]+\] .*\.$`
  - Example: `TODO [#33] This needs to be handled.` 
- `jira`: `^ TODO \[[a-zA-Z0-9]+\-[0-9]+\] .*\.$`
    - Example: `TODO [COMMON-1234] This needs to be handled.`

## Rules
| Rule ID | Category      | Severity | Url                 |
|---------|---------------|----------|---------------------|
| TA0001  | Documentation | Warning  | [TA0001](TA0001.md) |