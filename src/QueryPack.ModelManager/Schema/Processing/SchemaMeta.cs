namespace QueryPack.ModelManager.Schema.Processing
{
    using System.Text.Json.Serialization;

    public class SchemaMeta
    {
        [JsonPropertyName("product")]
        public string Product { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}