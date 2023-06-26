namespace QueryPack.ModelManager.Schema.Processing
{
    public record TranslationOptions(ISchemaResolver SchemaResolver, string RootNamespace, IEnumerable<string> AdditionalSources);
    public record ProcessingOptions(string RootNamesapce, string InstanceId);

}