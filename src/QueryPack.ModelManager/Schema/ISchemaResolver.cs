namespace QueryPack.ModelManager.Schema
{
    using Json.Schema;

    public interface ISchemaResolver
    {
        JsonSchema Resolve(Uri id);
    }
}
