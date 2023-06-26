namespace QueryPack.ModelManager.Schema.Processing
{
    using Microsoft.CodeAnalysis;

    public record CompilationMeta(string ClassName, string Namespace, string SourceCode, SyntaxTree Syntax);

    public interface ICompilationMetaRegistry
    {
        void Register(string key, string src);

        void Register(string key, IEnumerable<string> sourceFiles);

        bool TryGet(string key, out IEnumerable<CompilationMeta> compileMetaCollection);

        IEnumerable<CompilationMeta> GetAll();

        IEnumerable<CompilationMeta> GetAll(string key);
    }
}