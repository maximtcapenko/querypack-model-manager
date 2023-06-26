namespace QueryPack.ModelManager.Schema.Processing
{
    using System.Runtime.Serialization;

    public enum Deployment
    {
        [EnumMember(Value = "complete")]
        Complete = 1,
        [EnumMember(Value = "incremental")]
        Incremental
    }
}
