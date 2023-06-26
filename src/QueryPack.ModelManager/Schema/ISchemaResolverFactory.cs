namespace QueryPack.ModelManager.Schema
{
    using Json.Schema;

    public record ResolveOptions(bool UseBundle);

    public interface ISchemaResolverFactory
    {
        ISchemaResolver CreateSchemaResolver(ResolveOptions options, params JsonSchema[] schemaa);
    }
}
