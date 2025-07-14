using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Requires service interfaces to reside in Abstractions namespace.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InterfaceNamespaceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0009";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Place interfaces in Abstractions",
            "Interface '{0}' should be declared in a namespace containing 'Abstractions'",
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
            context.RegisterSyntaxNodeAction(AnalyzeInterface, SyntaxKind.InterfaceDeclaration);
        }

        private static void AnalyzeInterface(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InterfaceDeclarationSyntax iface)
            {
                var ns = iface.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
                if (ns == null || !ns.Name.ToString().Contains("Abstractions"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, iface.Identifier.GetLocation(), iface.Identifier.Text));
                }
            }
        }
    }
}
