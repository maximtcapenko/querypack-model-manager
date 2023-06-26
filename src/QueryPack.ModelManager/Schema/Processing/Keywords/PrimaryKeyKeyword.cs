namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword("$primaryKey")]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(BaseKeyKeywordConverter<PrimaryKeyKeyword>))]
    internal class PrimaryKeyKeyword : BaseKeyKeyword,
        IJsonSchemaKeyword, IEquatable<PrimaryKeyKeyword>
    {
        public PrimaryKeyKeyword(bool value) : base(value)
        { }

        public override string Name => "$primaryKey";

        public bool Equals(PrimaryKeyKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PrimaryKeyKeyword);
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}