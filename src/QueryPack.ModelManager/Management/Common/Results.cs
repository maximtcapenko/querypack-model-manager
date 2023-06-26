namespace QueryPack.ModelManager.Management.Common
{
    using Json.Schema;
    using Schema;

    public record ProcessingResult()
    {
        public virtual bool HasErrors { get; }
    }

    public record Success() : ProcessingResult
    {
        public override bool HasErrors => false;
    }

    public record Failure(ValidationResult ValidationResult) : ProcessingResult
    {
        public override bool HasErrors => true;
    }

    public record SchemaParsingResult(bool IsValid,
    SchemaKey SchemaKey, JsonSchema JsonSchema, ISchemaResolver SchemaResolver, ValidationResult ValidationResult);
}