using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.StrongTypingAnalyzer, CodingStandards.Analyzers.Rules.StrongTypingCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class StrongTypingAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAndFixAsync()
        {
            var testCode = "class C { dynamic d; }";
            var expected = VerifyCS.Diagnostic(StrongTypingAnalyzer.DiagnosticId).WithArguments("dynamic").WithSpan(1, 11, 1, 18);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
