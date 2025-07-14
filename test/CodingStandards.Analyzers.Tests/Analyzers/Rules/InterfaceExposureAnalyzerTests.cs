using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.InterfaceExposureAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class InterfaceExposureAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "public class Service {}";
            var expected = VerifyCS.Diagnostic(InterfaceExposureAnalyzer.DiagnosticId).WithArguments("Service").WithSpan(1, 14, 1, 21);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
