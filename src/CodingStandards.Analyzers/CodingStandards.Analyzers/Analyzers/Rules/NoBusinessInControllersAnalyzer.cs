using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Detects business logic placed in controllers or middleware.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NoBusinessInControllersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0006";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "No business logic in controllers",
            "Method '{0}' should delegate to a service instead of containing logic",
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
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax method && method.Body != null)
            {
                var cls = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (cls != null && (cls.Identifier.Text.EndsWith("Controller") || cls.Identifier.Text.EndsWith("Middleware")))
                {
                    if (method.Body.Statements.Count > 1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
                    }
                }
            }
        }
    }
}
