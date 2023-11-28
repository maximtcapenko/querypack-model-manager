namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Properties
{
    using System.Text;
    using Json.Schema;
    using Keywords;

    internal class InlineSchemaPropertyProcessor : IPropertySchemaProcessor
    {
        private static readonly IEnumerable<Type> _supportedAnnotations = new List<Type>
        {
            typeof(ForeignKeyKeyword),
            typeof(ColumnKeyword),
            typeof(PrimaryKeyKeyword),
            typeof(NotNullKeyword),
            typeof(MinLengthKeyword),
            typeof(MaxLengthKeyword),
            typeof(IdentityKeyword)
        };
        private readonly IEnumerable<IAnnotationProcessor> _annotationProcessors;

        public InlineSchemaPropertyProcessor(IEnumerable<IAnnotationProcessor> annotationProcessors)
        {
            _annotationProcessors = annotationProcessors;
        }

        public void Process(PropertyProcessingContext context)
        {
            var propertyType = ClrTypeResolver.ResolvePropertyType(context.CurrentSchema);
            if (propertyType == null) return;

            var annotatonBuilder = new StringBuilder();
            var annotationProcessingContext = new AnnotationProcessingContext(context.CurrentSchema, annotatonBuilder);

            foreach (var annotationProcessor in _annotationProcessors.Where(e => AnnotationsIsSupported(e)))
            {
                annotationProcessor.Process(annotationProcessingContext);
            }

            context.AppendProperty(propertyType, context.Name, annotatonBuilder);
        }

        private static bool AnnotationsIsSupported(IAnnotationProcessor processor)
            => _supportedAnnotations.Any(e => processor.CanProcess(e));
    }
}