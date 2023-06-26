namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Keywords;

    internal class ColumnNameAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(ColumnKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<ColumnKeyword>(out var column))
            {
                var columnAttribute = $"{nameof(ColumnAttribute)}(\"{column.Value}\")";
                context.Annotate(columnAttribute);
            }
        }
    }
}