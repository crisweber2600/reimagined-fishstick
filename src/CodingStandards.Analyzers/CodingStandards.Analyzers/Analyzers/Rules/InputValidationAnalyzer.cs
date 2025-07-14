using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Flags manual input validation in controllers.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InputValidationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0008";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use dedicated validators",
            "Input validation should be performed using validator classes",
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
            context.RegisterSyntaxNodeAction(AnalyzeIf, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIf(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax ifs)
            {
                var cls = ifs.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (cls != null && cls.Identifier.Text.EndsWith("Controller"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, ifs.IfKeyword.GetLocation()));
                }
            }
        }
    }
}
