namespace QueryPack.ModelManager.Management.Persistance
{
    public class BaseModel
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}