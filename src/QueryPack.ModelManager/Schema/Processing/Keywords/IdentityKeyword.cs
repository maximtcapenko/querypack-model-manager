namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword("$identity")]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(BaseKeyKeywordConverter<IdentityKeyword>))]
    internal class IdentityKeyword : BaseKeyKeyword,
        IJsonSchemaKeyword, IEquatable<IdentityKeyword>
    {
        public IdentityKeyword(bool value) : base(value)
        { }

        public override string Name => "$identity";

        public bool Equals(IdentityKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdentityKeyword);
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}