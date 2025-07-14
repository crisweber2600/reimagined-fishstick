using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Enforces constructor injection for dependencies.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ConstructorInjectionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0003";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use constructor injection",
            "Property injection is not allowed. Use constructor injection.",
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
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PropertyDeclarationSyntax prop)
            {
                if (prop.AccessorList?.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration) == true &&
                    prop.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, prop.GetLocation()));
                }
            }
        }
    }
}
