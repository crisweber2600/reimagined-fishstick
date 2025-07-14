using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.BddTestNamingAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class BddTestNamingAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "using System; public class FactAttribute : Attribute {} [Fact] public class Tests { public void Test() { } }";
            var expected = VerifyCS.Diagnostic(BddTestNamingAnalyzer.DiagnosticId).WithArguments("Test").WithSpan(1, 97, 1, 101);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
