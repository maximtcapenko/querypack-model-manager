namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword(Name)]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(MetaKeywordJsonConverter))]
    internal class MetaKeyword : IJsonSchemaKeyword, IEquatable<MetaKeyword>
    {
        public const string Name = "$meta";
        public SchemaMeta Value { get; }

        public MetaKeyword(SchemaMeta value)
        {
            Value = value;
        }

        public bool Equals(MetaKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public void Evaluate(EvaluationContext context)
        {
        }

        class MetaKeywordJsonConverter : JsonConverter<MetaKeyword>
        {
            public override MetaKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected object");

                var value = JsonSerializer.Deserialize<SchemaMeta>(ref reader, options)!;
                return new MetaKeyword(value);
            }

            public override void Write(Utf8JsonWriter writer, MetaKeyword value, JsonSerializerOptions options)
            {
                writer.WritePropertyName(Name);
                writer.WriteStartObject();
                JsonSerializer.Serialize(writer, value, options);
                writer.WriteEndObject();
            }
        }
    }
}