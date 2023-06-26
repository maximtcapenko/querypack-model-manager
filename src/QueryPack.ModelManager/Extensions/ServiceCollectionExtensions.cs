namespace QueryPack.ModelManager.Extensions
{
    using Schema;
    using Schema.Impl;
    using Schema.Processing;
    using Schema.Processing.Impl;
    using Schema.Processing.Keywords;
    using Schema.Processing.Processors;
    using Schema.Processing.Processors.Impl;
    using Schema.Processing.Processors.Impl.Annotations;
    using Schema.Processing.Processors.Impl.Properties;
    using Json.Schema;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaProcessingExtensions(this IServiceCollection self)
        {
            SchemaKeywordRegistry.Register<PrimaryKeyKeyword>();
            SchemaKeywordRegistry.Register<NotNullKeyword>();
            SchemaKeywordRegistry.Register<ColumnKeyword>();
            SchemaKeywordRegistry.Register<ForeignKeyKeyword>();
            SchemaKeywordRegistry.Register<TableKeyword>();
            SchemaKeywordRegistry.Register<IdentityKeyword>();
            SchemaKeywordRegistry.Register<IndexesKeyword>();
            SchemaKeywordRegistry.Register<MetaKeyword>();
            SchemaKeywordRegistry.Register<DeploymentKeyword>();

            Formats.Register(CustomFormats.Int64);
            Formats.Register(CustomFormats.Int32);
            Formats.Register(CustomFormats.Decimal);
            Formats.Register(CustomFormats.Float);
            Formats.Register(CustomFormats.Double);
            Formats.Register(CustomFormats.Guid);

            self.AddSingleton<ICompilationService, CSharpCompilationServiceImpl>()
            .AddTransient<ISchemaParser, JsonSchemaParserImpl>()
            .AddTransient<IJsonSchemaProcessor, JsonSchemaMigrationProcessorImpl>()
            .AddTransient<ISchemaResolverFactory, SchemaResovlerFactory>();

            return self;
        }

        internal static IServiceCollection AddSchemaInternalProcessing(this IServiceCollection self)
        {
            self.AddSingleton<IAnnotationProcessor, ColumnNameAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, ForeignKeyAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, NotNullAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, MaxLengthAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, MinLengthAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, PrimaryKeyAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, TableAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, IndexAnnotationProcessor>();
            self.AddSingleton<IAnnotationProcessor, IdentityAnnotationProcessor>();

            self.AddSingleton<IPropertySchemaProcessor, InlineSchemaPropertyProcessor>();
            self.AddSingleton<IPropertySchemaProcessor, ReferencedSchemaPropertyProcessor>();
            self.AddSingleton<ISchemaProcessor, CSharpSchemaProcessorImpl>();

            return self;
        }
    }
}