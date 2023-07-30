# Todo.Analyzer
.NET analyzer for C# making sure that all TODO comments respect the same coding standards.

## Documentation
See the [Documentation page](./Documentation/README.md) for setup and rules.
See the [Analyzer Releases](AnalyzerReleases.Shipped.md) for history of rules being shipped.

## Running tests and report code coverage
From the root of the repository run:
```powershell
./Scripts/RunTests.ps1
```
Code coverage will be reported as html (`./tests-and-coverage/index.html`), xml (`./tests-and-coverage/Summary.xml`) and markdown (`./tests-and-coverage/Summary.md`).

## Build a new package version
- Update `Todo.Analyzer.Package` project with the new version settings;
- Run `dotnet build -c Release ./Todo.Analyzer.Package/`;
- The NuGet package will be located in `./packages`;