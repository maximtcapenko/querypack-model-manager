namespace QueryPack.ModelManager.Schema.Processing.Extensions
{
    using Json.Schema;

    public static class SchemaUtils
    {
        public static string GetVersion(JsonSchema self)
            => InternalSchemaExtensions.GetMeta(self).Value.Version;

        public static string GetProduct(JsonSchema self)
            => InternalSchemaExtensions.GetMeta(self)?.Value.Product;

    }
}
