using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.ThinStartupAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class ThinStartupAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var lines = string.Join("\n", new string[61]);
            var expected = VerifyCS.Diagnostic(ThinStartupAnalyzer.DiagnosticId).WithSpan("Program.cs", 1, 1, 1, 1);
            await new VerifyCS.Test
            {
                TestState = { Sources = { ("Program.cs", lines) } },
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
