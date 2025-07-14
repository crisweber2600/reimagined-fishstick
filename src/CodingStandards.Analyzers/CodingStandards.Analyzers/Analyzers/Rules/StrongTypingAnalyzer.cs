using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeActions;
using System.Threading;
using System.Threading.Tasks;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Analyzer to prohibit usage of dynamic or object types.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class StrongTypingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0001";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            title: "Use strong typing",
            messageFormat: "Type '{0}' is not allowed; use a strongly typed alternative",
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.IdentifierName, SyntaxKind.PredefinedType);
        }

        private static void AnalyzeType(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.ObjectKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, pts.GetLocation(), "object"));
            }
            else if (context.Node is IdentifierNameSyntax ins && ins.Identifier.Text == "dynamic")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, ins.GetLocation(), "dynamic"));
            }
        }
    }

    /// <summary>
    /// Code fix provider for <see cref="StrongTypingAnalyzer"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StrongTypingCodeFixProvider))]
    public sealed class StrongTypingCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StrongTypingAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node is IdentifierNameSyntax or PredefinedTypeSyntax)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Replace with object",
                            ct => ReplaceWithObjectAsync(context.Document, node, ct),
                            equivalenceKey: "ReplaceWithObject"),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ReplaceWithObjectAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var objectType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
            var newRoot = root.ReplaceNode(node, objectType.WithTriviaFrom(node));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
