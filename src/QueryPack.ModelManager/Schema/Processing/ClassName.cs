namespace QueryPack.ModelManager.Schema.Processing
{
    public record ClassName(string ClsName, string Namespace)
    {
        public string FullName => $"{Namespace}.{ClsName}";
    }
}