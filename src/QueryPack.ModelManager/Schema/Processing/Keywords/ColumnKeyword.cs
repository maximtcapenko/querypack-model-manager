namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json.Serialization;
    using Json.Schema;

    [SchemaKeyword("$column")]
    [SchemaSpecVersion(SpecVersion.Draft6)]
    [SchemaSpecVersion(SpecVersion.Draft7)]
    [SchemaSpecVersion(SpecVersion.Draft201909)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [SchemaSpecVersion(SpecVersion.DraftNext)]
    [JsonConverter(typeof(ValueStringKeywordConverter<ColumnKeyword>))]
    internal class ColumnKeyword : ValueStringBaseKeyword,
       IJsonSchemaKeyword, IEquatable<ColumnKeyword>
    {
        public override string Name => "$column";

        public ColumnKeyword(string table) : base(table)
        {
            Value = table;
        }

        public override void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            context.LocalResult.SetAnnotation(Name, Value);
            context.ExitKeyword(Name, true);
        }

        public bool Equals(ColumnKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ColumnKeyword);
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}