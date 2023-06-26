namespace QueryPack.ModelManager.Schema.Processing.Impl
{
    using Json.Schema;
    using Management.Common;
    using Management.Extensions;
    using Management.Persistance;
    using Microsoft.EntityFrameworkCore;
    using Processing.Extensions;

    internal class JsonSchemaParserImpl : ISchemaParser
    {
        private readonly ISchemaResolverFactory _schemaResolverFactory;
        private readonly PersistanceContext _persistanceContext;

        private static List<Action<JsonSchema, List<string>>> _rootValidators
            = new List<Action<JsonSchema, List<string>>>
            {
                IdIsRequired,
                TypeIsRequired,
                TableIsRequired,
                PropertiesIsRequired,
                PrimaryKeyIsRequired,
                IndexShouldBeValid,
            };

        private static List<Action<JsonSchema, List<string>>> _propertyValidators
            = new List<Action<JsonSchema, List<string>>>()
            {
                TypeIsRequired,
                //ColumnIsRequired
            };

        public JsonSchemaParserImpl(ISchemaResolverFactory schemaResolverFactory,
            PersistanceContext persistanceContext)
        {
            _persistanceContext = persistanceContext;
            _schemaResolverFactory = schemaResolverFactory;
        }

        public async Task<SchemaParsingResult> ParseAsync(string jsonSchema)
        {
            var dbSchemas = await _persistanceContext.Schemas
                .Where(e => e.IsCurrent).ToListAsync();

            var dbJsonSchemas = dbSchemas.Select(e => JsonSchema.FromText(e.JsonSchema)).ToArray();
            var schema = JsonSchema.FromText(jsonSchema);
            var schemaResover = _schemaResolverFactory.CreateSchemaResolver(new ResolveOptions(false), dbJsonSchemas);

            /*
            foreach (var def in schema.GetDefinitions())
            {
                var id = def.Value.GetId();
                // validate if schema already registered
                var candidate = schemaResover.Resolve(id);
                if (candidate != null)
                {
                    var candidateKey = candidate.GetSchemaKey();
                    var currentKey = schema.GetSchemaKey();

                    if (candidateKey == currentKey)
                    {
                        var validateResult = new ValidationResult();
                        validateResult["id"] = "schema already registered and can't be processed";

                        return new SchemaParsingResult(false, null, null, null, validateResult);
                    }
                }
            }
            */


            // recreate resolver
            var dbJsonSchemasAndCurrent = dbJsonSchemas.Concat(new[] { schema }).ToArray();
            schemaResover = _schemaResolverFactory.CreateSchemaResolver(new ResolveOptions(false), dbJsonSchemasAndCurrent);
            var validationResult = new ValidationResult();

            foreach (var def in schema.GetDefinitions())
            {
                Validate(def.Value, schemaResover, validationResult);
            }
            if (validationResult.Count > 0)
                return new SchemaParsingResult(false, null, null, null, validationResult);


            return new SchemaParsingResult(true, schema.GetSchemaKey(), schema, schemaResover, null);
        }

        private void Validate(JsonSchema schema, ISchemaResolver resolver, ValidationResult validationResult)
        {
            var rootValidationMessages = new List<string>();

            _rootValidators.ForEach(validator => validator(schema, rootValidationMessages));
            if (rootValidationMessages.Count > 0)
                validationResult[ResolveIdentifier(schema)] = rootValidationMessages;

            var properties = schema.GetProperties();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var reference = property.Value.GetRef();
                    if (reference != null)
                    {
                        var targetSchema = resolver.Resolve(reference);
                        if (targetSchema != null)
                            Validate(targetSchema, resolver, validationResult);
                        else
                            validationResult[$"{ResolveIdentifier(schema)}.{property.Key}"] = $"Null reference of [{reference}]";
                    }
                    else
                    {
                        var propertyValidationMessages = new List<string>();
                        _propertyValidators.ForEach(validator => validator(property.Value, propertyValidationMessages));

                        if (propertyValidationMessages.Count > 0)
                            validationResult[$"{ResolveIdentifier(schema)}.{property.Key}"] = propertyValidationMessages;
                    }
                }
            }
        }

        private string ResolveIdentifier(JsonSchema schema)
        {
            var title = schema.GetTitle();
            if (title != null) return title;

            var table = schema.GetTable();
            if (!string.IsNullOrEmpty(table?.Value)) return table.Value;

            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private static void TypeIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var type = schema.GetJsonType();
            if (type == null)
                validateResult.Add("type is required but not defined");
        }

        private static void MetaIsRequired(JsonSchema schema, List<string> validateResult)
        {
            if (!schema.IsResourceRoot) return;

            var meta = schema.GetMeta();
            if (meta == null)
                validateResult.Add("$meta is required but not defined");
            else
            {
                VersionIsRequired(meta.Value, validateResult);
                ProductIdIsRequired(meta.Value, validateResult);
            }
        }

        private static void ColumnIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var column = schema.GetColumn();
            if (column == null)
                validateResult.Add("$column is required but not defined");
        }

        private static void TableIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var table = schema.GetTable();
            if (string.IsNullOrEmpty(table?.Value))
                validateResult.Add("$table is required but not defined");
        }


        private static void PrimaryKeyIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var primaryKey = schema.GetPrimaryKey();
            if (primaryKey == null)
                validateResult.Add("$primaryKey is required but not defined");
        }

        private static void PropertiesIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var properties = schema.GetProperties();
            if (properties == null)
                validateResult.Add("properties is required but not defined");
        }

        private static void IdIsRequired(JsonSchema schema, List<string> validateResult)
        {
            var id = schema.GetId();
            if (id == null)
                validateResult.Add("$id is required but not defined");
        }

        private static void ProductIdIsRequired(SchemaMeta meta, List<string> validateResult)
        {
            if (string.IsNullOrEmpty(meta.Product))
                validateResult.Add("$meta.product is required but not defined");
        }

        private static void VersionIsRequired(SchemaMeta meta, List<string> validateResult)
        {
            if (string.IsNullOrEmpty(meta.Version))
                validateResult.Add("$meta.version is required but not defined");
        }

        private static void IndexShouldBeValid(JsonSchema schema, List<string> validateResult)
        {
            var indexes = schema.GetIndexes()?.Indexes;
            var properties = schema.GetProperties();

            if (indexes != null && properties != null)
            {
                foreach (var index in indexes)
                {
                    var indexProperties = properties.Where(e => index.Value.Contains(e.Key));
                    if (indexProperties?.Count() == 0 || indexProperties.Count() != index.Value.Count())
                    {
                        validateResult.Add("$indexes is presented but index properties are not specified or do not exist");
                    }
                    else
                        foreach (var indexProperty in indexProperties)
                        {
                            var @ref = indexProperty.Value.GetRef();
                            if (@ref != null)
                            {
                                validateResult.Add($"property [{indexProperty.Key}] specified for index usage but it has reference, property should be inline");
                            }
                        }
                }
            }
        }
    }
}