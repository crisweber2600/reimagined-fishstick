using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Ensures services are exposed behind an interface with I prefix.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InterfaceExposureAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0004";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Expose service via interface",
            "Public class '{0}' should be accessed via an interface starting with 'I'",
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
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax cls && cls.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                var implements = cls.BaseList?.Types.Select(t => context.SemanticModel.GetTypeInfo(t.Type).Type as INamedTypeSymbol);
                if (implements is null || !implements.Any(t => t?.TypeKind == TypeKind.Interface && t.Name.StartsWith("I")))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, cls.Identifier.GetLocation(), cls.Identifier.Text));
                }
            }
        }
    }
}
