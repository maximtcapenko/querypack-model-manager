namespace QueryPack.ModelManager.Schema.Processing
{
    using Json.Schema;
    
    public record TranslationResult(IEnumerable<CompilationMeta> TranlsatedFiles, IEnumerable<CompilationMeta> AdditionalFiles);

    public interface ISchemaTranslator
    {
        TranslationResult Translate(JsonSchema schema);
    }
}