namespace QueryPack.ModelManager.Schema
{
    using Management.Common;
    using OneOf;

    public interface IJsonSchemaProcessor
    {
        Task<OneOf<Success, Failure>> ProcessAsync(string json);
    }
}