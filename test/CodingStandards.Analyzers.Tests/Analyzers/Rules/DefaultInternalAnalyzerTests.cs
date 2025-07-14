using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.DefaultInternalAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class DefaultInternalAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAsync()
        {
            var testCode = "public class Foo { public void Bar() {} }";
            var expected1 = VerifyCS.Diagnostic(DefaultInternalAnalyzer.DiagnosticId).WithArguments("Foo").WithSpan(1, 14, 1, 17);
            var expected2 = VerifyCS.Diagnostic(DefaultInternalAnalyzer.DiagnosticId).WithArguments("Bar").WithSpan(1, 32, 1, 35);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected1, expected2 }
            }.RunAsync();
        }
    }
}
