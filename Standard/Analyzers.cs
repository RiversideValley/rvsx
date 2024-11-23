﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Riverside.Standard.Helpers;
using System.Collections.Immutable;

namespace Riverside.Standard
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzers : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RX00001";
        private static readonly LocalizableString Title = "Class must derive from Riverside.Runtime.Modern.UnifiedApp";
        private static readonly LocalizableString MessageFormat = "Class '{0}' must derive from Riverside.Runtime.UnifiedApp";
        private static readonly LocalizableString Description = "Ensure that App class derives from Riverside.Runtime.UnifiedApp.";
        private const string Category = "Naming";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Check if the class is named "App"
            if (classDeclaration.Identifier.Text == "App")
            {
                // Check if the class derives from "Microsoft.UI.Xaml.Application"
                TypeSyntax baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
                if (baseType == null || !ClassInteractions.DerivesFrom(baseType, "Microsoft.UI.Xaml.Application", context.SemanticModel))
                {
                    return;
                }

                // Check if the class derives from "Riverside.Runtime.Modern.UnifiedApp"
                if (!ClassInteractions.DerivesFrom(baseType, "Riverside.Runtime.Modern.UnifiedApp", context.SemanticModel))
                {
                    Diagnostic diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
