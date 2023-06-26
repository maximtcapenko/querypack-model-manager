namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using Json.Schema;
    using Keywords;

    internal class IndexAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(IndexesKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            var indexFields = new List<string>();

            if (context.Schema.TryGetKeyword<IndexesKeyword>(out var index))
            {
                var properties = context.Schema.GetProperties();
                foreach (var key in index.Indexes)
                {
                    foreach (var candidate in key.Value)
                    {
                        indexFields.Add(candidate);
                    }
                }

                var parameters = string.Join(",", indexFields.Select(e => $"\"{e}\""));
                var indexAttribute = $"IndexAttribute({parameters})";
                context.Annotate(indexAttribute);
            }
        }
    }
}