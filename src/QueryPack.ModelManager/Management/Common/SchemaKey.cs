namespace QueryPack.ModelManager.Management.Common
{
    public record SchemaKey(string ProductId, string Version);

    public class ValidationResult : Dictionary<string, object>
    { }
}