namespace QueryPack.ModelManager.Api.Controllers
{
    using System.Text.Json;
    using Json.Schema;
    using Management.Persistance;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Schema;

    [Route("api/v1")]
    [ApiController]
    public class SchemasController : ControllerBase
    {
        private readonly PersistanceContext _persistanceContext;
        private readonly IJsonSchemaProcessor _jsonSchemaProcessor;
        private readonly ISchemaResolverFactory _schemaResolverFactory;

        public SchemasController(IJsonSchemaProcessor jsonSchemaProcessor,
            ISchemaResolverFactory schemaResolverFactory,
            PersistanceContext persistanceContext)
        {
            _jsonSchemaProcessor = jsonSchemaProcessor;
            _persistanceContext = persistanceContext;
            _schemaResolverFactory = schemaResolverFactory;
        }


        [HttpGet]
        [Route("schemas")]
        public async Task<IActionResult> GetSchema([FromQuery] Uri id, bool includeReferences = false)
        {
            var schemas = await _persistanceContext.Schemas.Where(e => e.IsCurrent).ToListAsync();
            var resovler = _schemaResolverFactory.CreateSchemaResolver(new ResolveOptions(includeReferences),
                schemas.Select(e => JsonSchema.FromText(e.JsonSchema)).ToArray());

            var schema = resovler.Resolve(id);
            if (schema == null)
            {
                return NotFound();
            }

            return Ok(schema);
        }


        [HttpPost]
        [Route("schemas")]
        public async Task<IActionResult> CreateSchema([FromBody] JsonDocument json)
        {
            var jsonString = JsonSerializer.Serialize(json);
            var result = await _jsonSchemaProcessor.ProcessAsync(jsonString);
            return result.Match<IActionResult>(Ok, BadRequest);
        }
    }
}
