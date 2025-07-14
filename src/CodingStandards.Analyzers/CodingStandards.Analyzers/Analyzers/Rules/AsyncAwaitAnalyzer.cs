using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Disallows synchronous wait of async operations.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AsyncAwaitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0011";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use async/await",
            "Avoid synchronous wait on Task; use await",
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
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax member)
            {
                if (member.Name.Identifier.Text == "Result")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, member.Name.GetLocation()));
                }
            }
            else if (context.Node is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax mae)
            {
                if (mae.Name.Identifier.Text == "Wait" && invocation.ArgumentList.Arguments.Count == 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, mae.Name.GetLocation()));
                }
            }
        }
    }
}
