namespace QueryPack.ModelManager.Schema.Processing.Extensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxTreeExtensions
    {
        public static string GetClassName(this SyntaxTree self)
        {
            var root = self.GetRoot() as CompilationUnitSyntax;
            var namespaceSyntax = root.Members.OfType<BaseNamespaceDeclarationSyntax>().First();
            return namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First().Identifier.ToString();
        }

        public static ClassDeclarationSyntax GetClassDeclaration(this SyntaxTree self)
        {
            var root = self.GetRoot() as CompilationUnitSyntax;
            var namespaceSyntax = root.Members.OfType<BaseNamespaceDeclarationSyntax>().First();
            return namespaceSyntax.Members.OfType<ClassDeclarationSyntax>().First();
        }

        public static string GetNamespace(this SyntaxTree self)
        {
            var root = self.GetRoot() as CompilationUnitSyntax;
            var namespaceSyntax = root.Members.OfType<BaseNamespaceDeclarationSyntax>().First();
            return namespaceSyntax.Name.ToString();
        }
    }
}
