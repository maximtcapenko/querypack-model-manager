namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword("$notNull")]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(BaseKeyKeywordConverter<NotNullKeyword>))]
    class NotNullKeyword : BaseKeyKeyword,
            IJsonSchemaKeyword, IEquatable<NotNullKeyword>
    {
        public NotNullKeyword() : base(true) { }

        public NotNullKeyword(bool value) : base(value)
        { }

        public override string Name => "$notNull";

        public bool Equals(NotNullKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NotNullKeyword);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}