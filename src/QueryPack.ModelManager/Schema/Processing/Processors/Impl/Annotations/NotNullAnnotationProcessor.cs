namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations;
    using Keywords;

    internal class NotNullAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(NotNullKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<NotNullKeyword>(out var required))
            {
                context.Annotate($"{nameof(RequiredAttribute)}");
            }
        }
    }
}