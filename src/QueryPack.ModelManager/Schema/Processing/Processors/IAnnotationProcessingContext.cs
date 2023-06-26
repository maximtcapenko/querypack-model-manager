namespace QueryPack.ModelManager.Schema.Processing.Processors
{
    using Json.Schema;

    public interface IAnnotationProcessingContext
    {
        JsonSchema Schema { get; }
        void Annotate(string annotation);
    }
}