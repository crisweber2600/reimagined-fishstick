# Common Solution

This repository contains a basic .NET solution with a library, an example application and unit tests. The library now includes a HTTP message handler that automatically refreshes expired bearer tokens.
It also supports initializing a `TasClient` using `TasClientBuilder` and registering it in dependency injection.
The client can now retrieve foundation information through `FoundationApi`.

## Projects
- **Common** - reusable library code.
- **Common.Example** - console application demonstrating library usage.
- **Common.UnitTests** - xUnit project containing all unit tests. Tests rely on `Moq` for mocking.

## Building
Run the build command before executing any tests. The `-tl:off` option
disables the terminal logger which can fail in non-interactive shells:

```bash
dotnet build -tl:off
```

## Running Tests
After building, execute the tests with the terminal logger disabled:

```bash
dotnet test -tl:off
```

The build step ensures that all projects compile successfully prior to running the test suite.
