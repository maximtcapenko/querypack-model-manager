namespace QueryPack.ModelManager.Schema.Processing.Processors
{
    using Json.Schema;
    using System.Text;

    public class PropertyProcessingContext
    {
        private readonly StringBuilder _typeBuilder;

        public PropertyProcessingContext(string propertyName, JsonSchema schema,
             StringBuilder typeBuilder,
             ISchemaResolver schemaResolver,
             ISchemaProcessor schemaProcessor)
        {
            Name = propertyName;
            CurrentSchema = schema;
            _typeBuilder = typeBuilder;
            SchemaResolver = schemaResolver;
            SchemaProcessor = schemaProcessor;
        }

        public string Name { get; }
        public ISchemaResolver SchemaResolver { get; }
        public JsonSchema CurrentSchema { get; }
        public ISchemaProcessor SchemaProcessor { get; }


        public void AppendProperty(string propertyType, string propertyName, StringBuilder annotationBuilder)
        {
            if (annotationBuilder.Length > 0)
                _typeBuilder.Append(annotationBuilder.ToString());

            _typeBuilder.AppendLine($"public {propertyType} {propertyName}" + " { get; set; }");
            _typeBuilder.AppendLine();
        }
    }
}