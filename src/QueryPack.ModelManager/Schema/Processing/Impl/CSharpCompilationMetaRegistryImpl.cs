namespace QueryPack.ModelManager.Schema.Processing.Impl
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Processing.Extensions;

    internal class CSharpCompilationMetaRegistryImpl : ICompilationMetaRegistry
    {
        record ClassSource(ClassName ClassName, CompilationMeta CompileMeta);

        private Dictionary<string, List<ClassSource>> _registry
            = new Dictionary<string, List<ClassSource>>();

        public IEnumerable<CompilationMeta> GetAll() => _registry.SelectMany(e => e.Value.Select(e => e.CompileMeta));

        public IEnumerable<CompilationMeta> GetAll(string key)
            => _registry.TryGetValue(key, out var sources) ? sources.Select(e => e.CompileMeta) : Enumerable.Empty<CompilationMeta>();

        public void Register(string key, string src)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(src);
            var namespaceName = syntaxTree.GetNamespace();
            var className = syntaxTree.GetClassName();

            var fullClsName = new ClassName(className, namespaceName);

            var classSource = new ClassSource(fullClsName, new CompilationMeta(className, namespaceName, src, syntaxTree));
            if (_registry.TryGetValue(key, out var classes))
            {
                // class should be unique
                if(classes.Any(e => e.ClassName == fullClsName))
                    return;
                    
                classes.Add(classSource);
            }
            else
                _registry.Add(key, new List<ClassSource> { classSource });
        }

        public void Register(string key, IEnumerable<string> sources)
        {
            foreach (var src in sources)
            {
                Register(key, src);
            }
        }

        public bool TryGet(string key, out IEnumerable<CompilationMeta> sources)
        {
            if (_registry.TryGetValue(key, out var srcs))
            {
                sources = srcs.Select(e => e.CompileMeta);
                return true;
            }
            else
            {
                sources = Enumerable.Empty<CompilationMeta>();
                return false;
            }
        }
    }
}