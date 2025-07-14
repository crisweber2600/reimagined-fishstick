using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Ensures exceptions are logged when caught.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ExceptionLoggingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0012";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Do not swallow exceptions",
            "Exceptions should be logged with contextual data",
            "Reliability",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeCatch, SyntaxKind.CatchClause);
        }

        private static void AnalyzeCatch(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CatchClauseSyntax cc && cc.Block != null)
            {
                var hasLog = cc.Block.DescendantNodes().OfType<InvocationExpressionSyntax>()
                    .Any(inv => inv.Expression.ToString().Contains("Log"));
                if (!hasLog)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, cc.CatchKeyword.GetLocation()));
                }
            }
        }
    }
}
