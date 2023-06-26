namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations;
    using Json.Schema;

    internal class MinLengthAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(MinLengthAttribute);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<MinLengthKeyword>(out var maxLength))
            {
                context.Annotate($"{nameof(MinLengthAttribute)}({maxLength.Value})");
            }
        }
    }
}