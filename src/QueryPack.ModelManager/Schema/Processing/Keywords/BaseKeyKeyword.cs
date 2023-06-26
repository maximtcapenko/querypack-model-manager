namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Json.Schema;

    internal class BaseKeyKeyword
    {
        public bool Value { get; }
        public virtual string Name { get; }

        public BaseKeyKeyword(bool value)
        {
            Value = value;
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            var scheamValueType = context.LocalInstance.GetSchemaValueType();
            var element = context.LocalInstance;
            //check type
            var type = element.GetSchemaValueType();
            if (type.HasFlag(SchemaValueType.Null))
            {
                context.WrongValueKind(scheamValueType);
                return;
            }
            if (type == SchemaValueType.Integer)
            {
                var value = element.GetValue<int>();
                if (value <= 0)
                {
                    context.LocalResult.Fail(Name, "Key should be greater then 0");
                }
            }
            if (type == SchemaValueType.String)
            {
                var value = element.GetValue<string>();
                if (string.IsNullOrEmpty(value))
                {
                    context.LocalResult.Fail(Name, "Key should not be empty string");
                }
            }

            context.ExitKeyword(Name, context.LocalResult.IsValid);
        }

        internal class BaseKeyKeywordConverter<T> : JsonConverter<T>
            where T : BaseKeyKeyword
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
                    throw new JsonException("Expected boolean");

                var value = reader.GetBoolean();
                var ctor = typeof(T).GetConstructor(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance, new[] { typeof(bool) });

                return (T)ctor.Invoke(new object[] { value });
            }
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteBoolean(value.Name, value.Value);
            }
        }
    }
}