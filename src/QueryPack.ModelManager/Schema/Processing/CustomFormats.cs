namespace QueryPack.ModelManager.Schema.Processing
{
    using Json.Schema;

    public class CustomFormats
    {
        public readonly static Format Int32 = new Format("int32");
        public readonly static Format Int64 = new Format("int64");
        public readonly static Format Decimal = new Format("decimal");
        public readonly static Format Double = new Format("double");
        public readonly static Format Float = new Format("float");
        public readonly static Format Guid = new Format("guid");
    }

}