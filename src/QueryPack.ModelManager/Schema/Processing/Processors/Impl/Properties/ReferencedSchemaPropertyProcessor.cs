namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Properties
{
    using System.Text;
    using Json.Schema;
    using Keywords;

    internal class ReferencedSchemaPropertyProcessor : IPropertySchemaProcessor
    {
        private static readonly IEnumerable<Type> _supportedAnnotations = new List<Type>
        {
            typeof(ForeignKeyKeyword),
            typeof(NotNullKeyword)
        };

        private readonly IEnumerable<IAnnotationProcessor> _annotationProcessors;

        public ReferencedSchemaPropertyProcessor(IEnumerable<IAnnotationProcessor> annotationProcessors)
        {
            _annotationProcessors = annotationProcessors;
        }

        public void Process(PropertyProcessingContext context)
        {
            if (context.CurrentSchema.TryGetKeyword<RefKeyword>(out var @ref))
            {
                var schema = context.SchemaResolver.Resolve(@ref.Reference);
                if (schema != null)
                {
                    var propertyType = context.SchemaProcessor.Process(schema);

                    var annotatonBuilder = new StringBuilder();
                    var annotationProcessingContext = new AnnotationProcessingContext(context.CurrentSchema, annotatonBuilder);

                    foreach (var annotationProcessor in _annotationProcessors.Where(e => AnnotationsIsSupported(e)))
                    {
                        annotationProcessor.Process(annotationProcessingContext);
                    }

                    context.AppendProperty(propertyType, context.Name, annotatonBuilder);
                }
            }
        }

        private static bool AnnotationsIsSupported(IAnnotationProcessor processor)
        => _supportedAnnotations.Any(e => processor.CanProcess(e));
    }
}