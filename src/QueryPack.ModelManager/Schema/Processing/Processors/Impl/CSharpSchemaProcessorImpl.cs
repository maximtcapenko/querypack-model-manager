namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl
{
    using System.Reflection;
    using System.Text;
    using Json.Schema;
    using Keywords;
    using Processing.Extensions;

    internal class CSharpSchemaProcessorImpl : ISchemaProcessor
    {
        private const string _classFullNameFormat = "{0}.{1}";
        private const string _classFormat = "public class {0}";
        private static List<string> _includes = new List<string>
        {
            "using System;",
            "using System.ComponentModel.DataAnnotations;",
            "using System.ComponentModel.DataAnnotations.Schema;",
            "using System.Collections.Generic;",
            "using Microsoft.EntityFrameworkCore;",
        };

        private readonly ProcessingOptions _options;
        private readonly ICompilationMetaRegistry _compilationMetaRegistry;
        private readonly ISchemaResolver _schemaResolver;
        private readonly IEnumerable<IAnnotationProcessor> _annotationProcessors;
        private readonly IEnumerable<IPropertySchemaProcessor> _propertyProcessors;
        private static IEnumerable<Type> _supportedAnnotations = new List<Type>
        {
            typeof(TableKeyword),
            typeof(IndexesKeyword)
        };

        public CSharpSchemaProcessorImpl(
            IEnumerable<IAnnotationProcessor> annotationProcessors,
            IEnumerable<IPropertySchemaProcessor> propertyProcessors,
            ICompilationMetaRegistry compilationMetaRegistry,
            ISchemaResolver schemaResolver,
            ProcessingOptions options)
        {
            _annotationProcessors = annotationProcessors;
            _propertyProcessors = propertyProcessors;
            _options = options;
            _compilationMetaRegistry = compilationMetaRegistry;
            _schemaResolver = schemaResolver;
        }

        // Has issue: stops processing if root type already registered
        public string Process(JsonSchema schema)
        {
            var tableName = schema.GetTable().Value;

            if (tableName == null) return null;

            var metaCollection = _compilationMetaRegistry.GetAll();

            var predicateExpression = new TableNameSyntaxPredicateExpression(tableName);
            var compiledMeta = metaCollection.FirstOrDefault(e => predicateExpression.Apply(e.Syntax));
            if (compiledMeta != null)
            {
                return string.Format(_classFullNameFormat, compiledMeta.Namespace, compiledMeta.ClassName);
            }

            var typeBuilder = new StringBuilder();

            foreach (var include in _includes)
            {
                typeBuilder.AppendLine(include);
            }

            typeBuilder.AppendLine();
            typeBuilder.AppendLine($"namespace {_options.RootNamesapce}");
            typeBuilder.AppendLine("{");
            typeBuilder.AppendLine();

            var annotationProcessingContext = new AnnotationProcessingContext(schema, typeBuilder);

            foreach (var annotationProcessor in _annotationProcessors.Where(e => AnnotationsIsSupported(e)))
            {
                annotationProcessor.Process(annotationProcessingContext);
            }

            typeBuilder.AppendLine(string.Format(_classFormat, tableName));
            typeBuilder.AppendLine("{");
            foreach (var property in schema.GetProperties())
            {
                var propertyProcessingContext = new PropertyProcessingContext(property.Key, property.Value,
                    typeBuilder, _schemaResolver, this);

                foreach (var propertyProcessor in _propertyProcessors)
                {
                    propertyProcessor.Process(propertyProcessingContext);
                }
            }

            typeBuilder.AppendLine("}");
            typeBuilder.AppendLine("}");

            var sourceCode = typeBuilder.ToString();
            _compilationMetaRegistry.Register(_options.InstanceId, sourceCode);

            return string.Format(_classFullNameFormat, _options.RootNamesapce, tableName);
        }

        private static bool AnnotationsIsSupported(IAnnotationProcessor processor)
           => _supportedAnnotations.Any(e => processor.CanProcess(e));
    }
}