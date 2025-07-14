using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Encourages internal over public visibility.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DefaultInternalAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0015";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Prefer internal",
            "Member '{0}' is public. Consider reducing visibility.",
            "Design",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeMember, SyntaxKind.ClassDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMember(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case ClassDeclarationSyntax cls when cls.Modifiers.Any(SyntaxKind.PublicKeyword):
                    context.ReportDiagnostic(Diagnostic.Create(Rule, cls.Identifier.GetLocation(), cls.Identifier.Text));
                    break;
                case MethodDeclarationSyntax method when method.Modifiers.Any(SyntaxKind.PublicKeyword):
                    context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
                    break;
            }
        }
    }
}
