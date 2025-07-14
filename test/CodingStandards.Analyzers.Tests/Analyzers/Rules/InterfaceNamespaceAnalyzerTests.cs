using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.InterfaceNamespaceAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class InterfaceNamespaceAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "namespace Services { public interface IMyService {} }";
            var expected = VerifyCS.Diagnostic(InterfaceNamespaceAnalyzer.DiagnosticId).WithArguments("IMyService").WithSpan(1, 39, 1, 49);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
