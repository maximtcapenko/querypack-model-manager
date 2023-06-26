namespace QueryPack.ModelManager.Schema.Processing.Extensions
{
    using Json.Schema;
    using Keywords;

    internal static class InternalSchemaExtensions
    {
        public static IndexesKeyword GetIndexes(this JsonSchema self)
            => self.TryGetKeyword<IndexesKeyword>(out var indexes) ? indexes : null;

        public static DeploymentKeyword GetDeploymnet(this JsonSchema self)
            => self.TryGetKeyword<DeploymentKeyword>(out var deployment) ? deployment : null;

        public static TableKeyword GetTable(this JsonSchema self)
            => self.TryGetKeyword<TableKeyword>(out var table) ? table : null;

        public static MetaKeyword GetMeta(this JsonSchema self)
            => self.TryGetKeyword<MetaKeyword>(out var meta) ? meta : null;

        public static ColumnKeyword GetColumn(this JsonSchema self)
            => self.TryGetKeyword<ColumnKeyword>(out var column) ? column : null;

        public static PrimaryKeyKeyword GetPrimaryKey(this JsonSchema self)
        {
            var properties = self.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var primaryKey = property.Value.GetPrimaryKey();
                    if (primaryKey != null)
                        return primaryKey;
                }
            }
            else
            {
                if (self.TryGetKeyword<PrimaryKeyKeyword>(out var primaryKey))
                    return primaryKey;
            }
            return null;
        }

        public static IReadOnlyDictionary<string, JsonSchema> GetInlineProperties(this JsonSchema self)
        {
            var properties = self.GetProperties();
            var result = new Dictionary<string, JsonSchema>();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var @ref = property.Value.GetRef();
                    if (@ref == null)
                        result[property.Key] = property.Value;
                }
            }

            return result;
        }


        public static IReadOnlyDictionary<string, ColumnKeyword> GetColumns(this JsonSchema self)
        {
            var properties = self.GetProperties();
            var result = new Dictionary<string, ColumnKeyword>();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var column = property.Value.GetColumn();
                    if (column != null)
                        result[property.Key] = column;
                }
            }

            return result;
        }
    }
}