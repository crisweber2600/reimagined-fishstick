using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.BusinessLogicServiceAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class BusinessLogicServiceAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "public class HomeController { void Index() { int x=0; x++; } }";
            var expected = VerifyCS.Diagnostic(BusinessLogicServiceAnalyzer.DiagnosticId).WithArguments("Index").WithSpan(1, 36, 1, 41);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
