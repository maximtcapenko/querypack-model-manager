namespace QueryPack.ModelManager.Schema.Processing.Processors
{
    public interface IAnnotationProcessor
    {
        bool CanProcess(Type annotation);
        void Process(IAnnotationProcessingContext context);
    }
}