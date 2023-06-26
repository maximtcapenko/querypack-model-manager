namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl.Annotations
{
    using System.ComponentModel.DataAnnotations;
    using Json.Schema;

    internal class MaxLengthAnnotationProcessor : IAnnotationProcessor
    {
        public bool CanProcess(Type annotation) => annotation == typeof(MaxLengthKeyword);

        public void Process(IAnnotationProcessingContext context)
        {
            if (context.Schema.TryGetKeyword<MaxLengthKeyword>(out var maxLength))
            {
                context.Annotate($"{nameof(MaxLengthAttribute)}({maxLength.Value})");
            }
        }
    }
}