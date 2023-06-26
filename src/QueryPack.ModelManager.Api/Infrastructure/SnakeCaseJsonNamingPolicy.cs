namespace QueryPack.ModelManager.Api.Infrastructure
{
    using System.Text.Json;
    using Humanizer;

    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.Underscore();
    }
}
