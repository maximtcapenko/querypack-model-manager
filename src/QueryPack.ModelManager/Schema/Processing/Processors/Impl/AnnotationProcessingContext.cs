namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl
{
    using System.Text;
    using Json.Schema;

    internal class AnnotationProcessingContext : IAnnotationProcessingContext
    {
        private readonly StringBuilder _propertyBuilder;

        public JsonSchema Schema { get; }

        public AnnotationProcessingContext(JsonSchema schema, StringBuilder propertyBuilder)
        {
            Schema = schema;
            _propertyBuilder = propertyBuilder;
        }

        public void Annotate(string attributeBuilder)
        {
            _propertyBuilder.AppendLine($"[{attributeBuilder}]");
        }
    }
}