namespace QueryPack.ModelManager.Schema.Processing
{
    using Json.Schema;
    using Microsoft.Extensions.DependencyInjection;
    using ModelManager.Extensions;
    using Processing.Impl;
    using Processing.Processors;


    public class CSharpSchemaTranslator : ISchemaTranslator
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly string _instanceId = Guid.NewGuid().ToString();
        private readonly string _additionalFilesId;

        public CSharpSchemaTranslator(TranslationOptions translationOptions)
        {
            _additionalFilesId = nameof(translationOptions.AdditionalSources);

            var compilationMetaRegistry = new CSharpCompilationMetaRegistryImpl();
            compilationMetaRegistry.Register(_additionalFilesId, translationOptions.AdditionalSources);

            var services = new ServiceCollection()
                .AddSchemaInternalProcessing()
                .AddSingleton<ICompilationMetaRegistry>(compilationMetaRegistry)
                .AddSingleton(translationOptions.SchemaResolver)
                .AddSingleton(new ProcessingOptions(translationOptions.RootNamespace, _instanceId));

            _internalServiceProvider = services.BuildServiceProvider();
        }

        public TranslationResult Translate(JsonSchema schema)
        {
            var classProcessor = _internalServiceProvider.GetRequiredService<ISchemaProcessor>();
            var defs = schema.GetDefinitions();

            foreach (var def in defs)
            {
                classProcessor.Process(def.Value);

            }
            
            var registry = _internalServiceProvider.GetRequiredService<ICompilationMetaRegistry>();

            return new TranslationResult(registry.GetAll(_instanceId), registry.GetAll(_additionalFilesId));
        }
    }
}