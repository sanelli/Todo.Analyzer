# Todo.Analyzer
C# analyzer making sure that all TODO comments have the same format in the code.

## Build a new package version
- Update `Todo.Analyzer.Package` project with the new version settings
- Run `dotnet build -c Release ./Todo.Analyzer.Package/`
- The NuGet package will be located in `./packages`