namespace QueryPack.ModelManager.Management.Extensions
{
    using Common;
    using Json.Schema;
    using Schema.Processing.Extensions;

    public static class CommonExtensions
    {
        public static SchemaKey GetSchemaKey(this JsonSchema self)
        {
            var meta = self.GetMeta();
            var productId = meta.Value.Product;
            var version = meta.Value.Version;

            if (productId == null && version == null) return null;

            return new SchemaKey(productId, version);
        }
    }
}
