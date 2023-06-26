namespace QueryPack.ModelManager.Schema.Processing.Processors
{
    using Json.Schema;

    public interface ISchemaProcessor
    {
        string Process(JsonSchema context);
    }
}