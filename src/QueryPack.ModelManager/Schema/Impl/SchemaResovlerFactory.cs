namespace QueryPack.ModelManager.Schema.Impl
{
    using Json.Schema;

    internal class SchemaResovlerFactory : ISchemaResolverFactory
    {
        public ISchemaResolver CreateSchemaResolver(ResolveOptions options, params JsonSchema[] schemas)
        {
            return new JsonSchemaResolverImpl(options, schemas);
        }
    }
}
