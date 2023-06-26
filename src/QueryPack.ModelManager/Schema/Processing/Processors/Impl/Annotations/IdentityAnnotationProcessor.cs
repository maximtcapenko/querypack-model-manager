namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Keywords;

    internal class IdentityAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(IdentityKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<IdentityKeyword>(out var identity))
            {
                var foreignKeyAttribute = $"{nameof(DatabaseGeneratedAttribute)}(DatabaseGeneratedOption.Identity)";
                context.Annotate(foreignKeyAttribute);
            }
        }
    }
}