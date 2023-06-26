namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Keywords;


    internal class ForeignKeyAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(ForeignKeyKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<ForeignKeyKeyword>(out var foreignKey))
            {
                var foreignKeyAttribute = $"{nameof(ForeignKeyAttribute)}(\"{foreignKey.Value}\")";
                context.Annotate(foreignKeyAttribute);
            }
        }
    }
}