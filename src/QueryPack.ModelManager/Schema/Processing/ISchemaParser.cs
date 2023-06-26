namespace QueryPack.ModelManager.Schema.Processing
{
    using Management.Common;

    public interface ISchemaParser
    {
        Task<SchemaParsingResult> ParseAsync(string jsonSchema);
    }
}