namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Properties
{
    using Json.Schema;

    internal class ClrTypeResolver
    {
        public static string ResolvePropertyType(JsonSchema schema)
        {
            if (schema.TryGetKeyword<TypeKeyword>(out var type))
            {
                if (type.Type == SchemaValueType.Object ||
                   type.Type == SchemaValueType.Array ||
                   type.Type == SchemaValueType.Null) return null;

                var format = TryResolveFormat(schema);
                if (type.Type == SchemaValueType.Integer && IsInt64(type.Type, format)) return typeof(long).FullName;
                if (type.Type == (SchemaValueType.Null | SchemaValueType.Integer) && IsInt64(type.Type, format)) return $"{typeof(long).FullName}?";

                if (type.Type == SchemaValueType.Integer) return typeof(int).FullName;
                if (type.Type == (SchemaValueType.Null | SchemaValueType.Integer)) return $"{typeof(int).FullName}?";

                if (type.Type == SchemaValueType.Boolean) return typeof(bool).FullName;
                if (type.Type == (SchemaValueType.Boolean | SchemaValueType.Null)) return $"{typeof(bool)}?";

                if (type.Type == SchemaValueType.Number) return typeof(decimal).FullName;
                if (type.Type == (SchemaValueType.Number | SchemaValueType.Null)) return $"{typeof(decimal)}?";

                if (type.Type == SchemaValueType.Number && IsDouble(type.Type, format)) return typeof(double).FullName;
                if (type.Type == (SchemaValueType.Number | SchemaValueType.Null) && IsDouble(type.Type, format)) return $"{typeof(double)}?";

                if (type.Type == SchemaValueType.String && IsDateTime(type.Type, format)) return typeof(DateTimeOffset).FullName;
                if (type.Type == (SchemaValueType.String | SchemaValueType.Null) && IsDateTime(type.Type, format)) return $"{typeof(DateTimeOffset)}?";

                if (type.Type == SchemaValueType.String && IsGuid(type.Type, format)) return typeof(Guid).FullName;
                if (type.Type == (SchemaValueType.String | SchemaValueType.Null) && IsGuid(type.Type, format)) return $"{typeof(Guid).FullName}?";

                if (type.Type == SchemaValueType.String) return typeof(string).FullName;
            }

            return null;
        }

        public static Format TryResolveFormat(JsonSchema schema)
        {
            if (schema.TryGetKeyword<FormatKeyword>(out var format)) return format.Value;
            else return null;
        }

        public static bool IsInt64(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.Integer && format == CustomFormats.Int64)
                return true;

            return false;
        }

        public static bool IsInt32(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.Integer && format == CustomFormats.Int32)
                return true;

            return false;
        }

        public static bool IsDecimal(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.Number && format == CustomFormats.Decimal)
                return true;

            return false;
        }

        public static bool IsDouble(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.Number && format == CustomFormats.Double)
                return true;

            return false;
        }

        public static bool IsFloat(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.Number && format == CustomFormats.Float)
                return true;

            return false;
        }

        public static bool IsGuid(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.String && (format == CustomFormats.Guid
            || format == Formats.Uuid))
                return true;

            return false;
        }

        public static bool IsDateTime(SchemaValueType valueType, Format format)
        {
            if (format == null) return false;
            if (valueType == SchemaValueType.String && format == Formats.DateTime)
                return true;

            return false;
        }
    }
}