namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword("$indexes")]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(IndexesKeywordJsonConverter))]
    internal class IndexesKeyword :
        IJsonSchemaKeyword, IEquatable<IndexesKeyword>
    {
        public IReadOnlyDictionary<string, IEnumerable<string>> Indexes { get; }

        public IndexesKeyword(Dictionary<string, IEnumerable<string>> value)
        {
            Indexes = value;
        }

        public const string Name = "$indexes";

        public bool Equals(IndexesKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Indexes == other.Indexes;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IndexesKeyword);
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            var schemaValueType = context.LocalInstance.GetSchemaValueType();
            if (schemaValueType != SchemaValueType.Object)
            {
                context.WrongValueKind(schemaValueType);
                return;
            }

            var obj = (JsonObject)context.LocalInstance;
            if (!obj.VerifyJsonObject()) return;

            context.Options.LogIndentLevel++;
            var notFound = new List<string>();
            foreach (var index in Indexes)
            {
                foreach (var property in index.Value)
                {
                    var property1 = property;
                    context.Log(() => $"Checking for property '{property1}'");
                    if (!obj.TryGetPropertyValue(property, out _))
                        notFound.Add(property);
                    if (notFound.Count != 0 && context.ApplyOptimizations) break;
                }
            }
            if (notFound.Any())
                context.Log(() => $"Missing properties: [{string.Join(",", notFound.Select(x => $"'{x}'"))}]");
            context.Options.LogIndentLevel--;

            if (notFound.Count != 0)
                context.LocalResult.Fail(Name, ErrorMessages.Required, ("missing", notFound));
            context.ExitKeyword(Name, context.LocalResult.IsValid);
        }

        class IndexesKeywordJsonConverter : JsonConverter<IndexesKeyword>
        {
            public override IndexesKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected object");

                var value = JsonSerializer.Deserialize<Dictionary<string, IEnumerable<string>>>(ref reader, options)!;
                return new IndexesKeyword(value);
            }

            public override void Write(Utf8JsonWriter writer, IndexesKeyword value, JsonSerializerOptions options)
            {
                writer.WritePropertyName(Name);
                writer.WriteStartObject();
                foreach (var kvp in value.Indexes)
                {
                    writer.WritePropertyName(kvp.Key);
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
        }

        public override int GetHashCode()
        {
            return Indexes.GetStringDictionaryHashCode();
        }
    }
}