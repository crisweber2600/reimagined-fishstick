using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Requires feature flags to be injected rather than read from configuration directly.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FeatureFlagAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0013";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Inject feature flags",
            "Feature flags should be injected, not read via Configuration[index]",
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
            context.RegisterSyntaxNodeAction(AnalyzeIndexer, SyntaxKind.ElementAccessExpression);
        }

        private static void AnalyzeIndexer(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ElementAccessExpressionSyntax ea && ea.Expression.ToString().Contains("Configuration"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, ea.GetLocation()));
            }
        }
    }
}
