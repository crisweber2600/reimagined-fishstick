using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using System.Threading;
using System.Threading.Tasks;

namespace CodingStandards.Analyzers.Rules
{
    /// <summary>
    /// Favors record types for DTOs.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseRecordForDtoAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSAD0007";
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use record for DTO",
            "DTO '{0}' should be declared as a record",
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
            if (context.Node is ClassDeclarationSyntax cls && cls.Identifier.Text.EndsWith("Dto"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, cls.Identifier.GetLocation(), cls.Identifier.Text));
            }
        }
    }

    /// <summary>
    /// Code fix provider for <see cref="UseRecordForDtoAnalyzer"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseRecordForDtoCodeFixProvider))]
    public sealed class UseRecordForDtoCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(UseRecordForDtoAnalyzer.DiagnosticId);

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
                if (node is ClassDeclarationSyntax cls)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Convert to record",
                            ct => ConvertToRecordAsync(context.Document, cls, ct),
                            equivalenceKey: "ConvertToRecord"),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ConvertToRecordAsync(Document document, ClassDeclarationSyntax cls, CancellationToken ct)
        {
            var recordDecl = SyntaxFactory.RecordDeclaration(
                attributeLists: cls.AttributeLists,
                modifiers: cls.Modifiers,
                keyword: SyntaxFactory.Token(SyntaxKind.RecordKeyword),
                identifier: cls.Identifier,
                typeParameterList: cls.TypeParameterList,
                parameterList: null,
                baseList: cls.BaseList,
                constraintClauses: cls.ConstraintClauses,
                openBraceToken: cls.OpenBraceToken,
                members: cls.Members,
                closeBraceToken: cls.CloseBraceToken,
                semicolonToken: cls.SemicolonToken);

            var root = await document.GetSyntaxRootAsync(ct).ConfigureAwait(false);
            var newRoot = root!.ReplaceNode(cls, recordDecl);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
