namespace QueryPack.ModelManager.Schema.Processing
{
    using Json.Schema;

    public class CustomFormats
    {
        public readonly static Format Int32 = new("int32");
        public readonly static Format Int64 = new("int64");
        public readonly static Format Decimal = new("decimal");
        public readonly static Format Double = new("double");
        public readonly static Format Float = new("float");
        public readonly static Format Guid = new("guid");
    }

}