namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations;
    using Keywords;

    internal class PrimaryKeyAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(PrimaryKeyKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<PrimaryKeyKeyword>(out var primaryKey))
            {
                context.Annotate($"{nameof(KeyAttribute)}");
            }
        }
    }
}