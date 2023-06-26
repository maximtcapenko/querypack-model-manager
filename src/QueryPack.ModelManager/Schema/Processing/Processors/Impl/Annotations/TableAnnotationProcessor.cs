namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Keywords;

    internal class TableAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(TableKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<TableKeyword>(out var meta))
            {
                var table = meta.Value;
                context.Annotate($"{nameof(TableAttribute)}(\"{table}\")");
            }
        }
    }
}