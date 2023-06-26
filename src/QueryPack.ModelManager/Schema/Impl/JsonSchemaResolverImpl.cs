namespace QueryPack.ModelManager.Schema.Impl
{
    using Json.Pointer;
    using Json.Schema;

    internal class JsonSchemaResolverImpl : ISchemaResolver
    {
        private readonly SchemaRegistry _schemaRegistry = new SchemaRegistry();
        private readonly IEnumerable<Uri> _searchPaths;
        private readonly ResolveOptions _resolveOptions;

        public JsonSchemaResolverImpl(ResolveOptions resolveOptions, IEnumerable<JsonSchema> schemas)
        {
            var searchPaths = new List<Uri>();

            foreach (var schema in schemas)
            {
                searchPaths.Add(schema.BaseUri);
                _schemaRegistry.Register(schema);
            }

            _searchPaths = searchPaths;
            _resolveOptions = resolveOptions;
        }

        public JsonSchema Resolve(Uri id)
        {
            foreach (var baseUri in _searchPaths)
            {
                var uri = new Uri(baseUri, id);
                var targetBase = _schemaRegistry.Get(uri);
                if (JsonPointer.TryParse(uri.Fragment, out var pointerFragment))
                {
                    var targetSchema = targetBase?.FindSubschema(pointerFragment, null);
                    if (targetSchema != null)
                    {
                        var founds = new Dictionary<Uri, JsonSchema>();
                        foreach (var property in targetSchema.GetProperties())
                            Fetch(property.Value, _schemaRegistry, founds);

                        if (founds.Count() > 0 && _resolveOptions.UseBundle)
                        {
                            var options = new EvaluationOptions();
                            founds.Values.ToList().ForEach(e => options.SchemaRegistry.Register(e));

                            return targetSchema.Bundle(options);
                        }
                        else
                            return targetSchema;
                    }
                }
            }
            return null;
        }

        private void Fetch(JsonSchema currentSchema, SchemaRegistry registry, Dictionary<Uri, JsonSchema> founds)
        {
            if (currentSchema.TryGetKeyword<RefKeyword>(out var @ref))
            {
                var uri = new Uri(currentSchema.BaseUri, @ref.Reference);
                var baseUri = new Uri(uri.GetLeftPart(UriPartial.Query));
                var targetBase = registry.Get(baseUri);

                if (JsonPointer.TryParse(uri.Fragment, out var pointerFragment))
                {
                    var targetSchema = targetBase.FindSubschema(pointerFragment, null);
                    if (targetSchema != null)
                    {
                        if (!founds.ContainsKey(targetSchema.GetId()))
                            founds[targetSchema.GetId()] = targetSchema;

                        var properties = targetSchema.GetProperties();
                        if (properties != null)
                        {
                            foreach (var property in properties)
                                Fetch(property.Value, registry, founds);
                        }
                    }
                }
            }
        }
    }
}
