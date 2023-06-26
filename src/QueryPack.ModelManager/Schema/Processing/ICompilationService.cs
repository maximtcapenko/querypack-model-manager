namespace QueryPack.ModelManager.Schema.Processing
{
    using System.Reflection;

    public interface ICompilationService
    {
        Assembly Compile(IEnumerable<string> sourceFiles, params Assembly[] referencedAssemblies);
    }
}