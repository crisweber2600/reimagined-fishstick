using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Ensures HttpClient usage is wrapped in typed clients.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class HttpClientEncapsulationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0002";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Encapsulate HttpClient",
            "HttpClient should only be used within classes implementing an I*Client interface",
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
            context.RegisterSyntaxNodeAction(AnalyzeNew, SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeNew(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ObjectCreationExpressionSyntax oces)
            {
                var type = context.SemanticModel.GetTypeInfo(oces).Type;
                if (type?.Name == "HttpClient")
                {
                    var classDecl = oces.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                    if (classDecl == null)
                    {
                        return;
                    }

                    var implementsClient = false;
                    foreach (var baseType in classDecl.BaseList?.Types ?? default)
                    {
                        var t = context.SemanticModel.GetTypeInfo(baseType.Type).Type as INamedTypeSymbol;
                        if (t?.TypeKind == TypeKind.Interface && t.Name.StartsWith("I") && t.Name.EndsWith("Client"))
                        {
                            implementsClient = true;
                            break;
                        }
                    }

                    if (!implementsClient)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, oces.GetLocation()));
                    }
                }
            }
        }
    }
}
