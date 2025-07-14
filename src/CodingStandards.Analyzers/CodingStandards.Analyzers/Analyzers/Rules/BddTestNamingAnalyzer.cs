using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Enforces BDD naming pattern for test methods.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class BddTestNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0010";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use BDD naming",
            "Test method '{0}' should follow Given_When_Then pattern",
            "Naming",
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
            if (context.Node is MethodDeclarationSyntax method && method.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                var cls = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (cls != null && cls.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString().Contains("Fact") || a.Name.ToString().Contains("Theory"))))
                {
                    if (!method.Identifier.Text.Contains("_"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
                    }
                }
            }
        }
    }
}
