using System.Threading.Tasks;
using CodingStandards.Analyzers.Rules;
using Xunit;
using VerifyCS = CodingStandards.Analyzers.Tests.CSharpVerifier<CodingStandards.Analyzers.Rules.UseRecordForDtoAnalyzer, CodingStandards.Analyzers.Rules.UseRecordForDtoCodeFixProvider>;

namespace CodingStandards.Analyzers.Tests.Analyzers.Rules
{
    public class UseRecordForDtoAnalyzerTests
    {
        [Fact]
        public async Task ReportsDiagnosticAndFixAsync()
        {
            var testCode = "class PersonDto {}";
            var fixedCode = "record PersonDto {}";
            var expected = VerifyCS.Diagnostic(UseRecordForDtoAnalyzer.DiagnosticId).WithArguments("PersonDto").WithSpan(1, 7, 1, 16);
            await new VerifyCS.Test
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { expected }
            }.RunAsync();
        }
    }
}
