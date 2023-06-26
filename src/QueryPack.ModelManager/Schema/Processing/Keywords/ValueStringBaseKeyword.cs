namespace QueryPack.ModelManager.Schema.Processing.Keywords
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Json.Schema;

    internal class ValueStringBaseKeyword
    {
        public string Value { get; set; }
        public virtual string Name { get; }

        public ValueStringBaseKeyword(string name)
        {
            Value = name;
        }

        public virtual void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            var scheamValueType = context.LocalInstance.GetSchemaValueType();
            var element = context.LocalInstance;
            //check type
            var type = element.GetSchemaValueType();
            if (type != SchemaValueType.String)
            {
                context.WrongValueKind(scheamValueType);
                return;
            }

            var value = element.GetValue<string>();
            if (string.IsNullOrEmpty(value))
            {
                context.LocalResult.Fail(Name, $"{Name} should not be empty string");
            }

            context.ExitKeyword(Name, context.LocalResult.IsValid);

        }

        internal class ValueStringKeywordConverter<T> : JsonConverter<T>
           where T : ValueStringBaseKeyword
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException("Expected boolean");

                var value = reader.GetString();

                var ctor = typeof(T).GetConstructor(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance, new[] { typeof(string) });

                return (T)ctor.Invoke(new object[] { value });
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteString(value.Name, value.Value);
            }
        }
    }
}