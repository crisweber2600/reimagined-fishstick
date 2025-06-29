# Common Solution

This repository contains a basic .NET solution with a library, an example application and unit tests.

## Projects
- **Common** - reusable library code.
- **Common.Example** - console application demonstrating library usage.
- **Common.UnitTests** - xUnit project containing all unit tests. Tests rely on `Moq` for mocking.

## Building
Run the build command before executing any tests:

```bash
dotnet build
```

## Running Tests
After building, execute:

```bash
dotnet test
```

The build step ensures that all projects compile successfully prior to running the test suite.
