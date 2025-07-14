using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Enforces short Program/Startup files.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ThinStartupAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0014";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Keep Program/Startup thin",
            "Program or Startup file should not exceed 60 lines",
            "Design",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxTreeAction(AnalyzeTree);
        }

        private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
        {
            var path = context.Tree.FilePath ?? string.Empty;
            if (path.EndsWith("Program.cs") || path.EndsWith("Startup.cs"))
            {
                var lines = context.Tree.GetText().Lines.Count;
                if (lines > 60)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, Location.Create(context.Tree, new TextSpan(0, 0))));
                }
            }
        }
    }
}
