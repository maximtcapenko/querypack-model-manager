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
    [JsonConverter(typeof(DeploymentKeywordConverter))]
    class DeploymentKeyword : IJsonSchemaKeyword, IEquatable<DeploymentKeyword>
    {
        public const string Name = "$deployment";
        public Deployment Value { get; }

        public DeploymentKeyword(Deployment value)
        {
            Value = value;
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            context.LocalResult.SetAnnotation(Name, Enum.GetName(Value));
            context.ExitKeyword(Name, true);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeploymentKeyword);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(DeploymentKeyword other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value == other.Value;
        }
        internal class DeploymentKeywordConverter : JsonConverter<DeploymentKeyword>
        {
            public override DeploymentKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var overrideOptions = new JsonSerializerOptions();
                overrideOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                var deployment = JsonSerializer.Deserialize<Deployment>(ref reader, overrideOptions);
                return new DeploymentKeyword(deployment);
            }

            public override void Write(Utf8JsonWriter writer, DeploymentKeyword value, JsonSerializerOptions options)
            {
                var overrideOptions = new JsonSerializerOptions();
                overrideOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                writer.WritePropertyName(Name);
                JsonSerializer.Serialize(writer, value.Value, overrideOptions);
            }
        }
    }
}
