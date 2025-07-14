using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.InputValidationAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class InputValidationAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "public class HomeController { void Post(string name) { if(name==null){} } }";
            var expected = VerifyCS.Diagnostic(InputValidationAnalyzer.DiagnosticId).WithSpan(1, 56, 1, 58);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
