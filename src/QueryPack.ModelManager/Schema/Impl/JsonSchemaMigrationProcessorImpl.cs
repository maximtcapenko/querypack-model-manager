namespace QueryPack.ModelManager.Schema.Impl
{
    using System.Reflection;
    using Management.Common;
    using Management.Persistance;
    using Microsoft.CodeAnalysis;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Scaffolding;
    using OneOf;
    using Processing;
    using Services;

    internal class JsonSchemaMigrationProcessorImpl : IJsonSchemaProcessor
    {
        private readonly ICompilationService _compilationService;
        private readonly PersistanceContext _persistanceContext;
        private readonly ISchemaParser _schemaParser;
        private readonly IScaffoldService _scaffoldService;

        const string scaffoldedContextClassName = "ScaffoldedContext";
        const string compiledContextClassName = "CompiledContext";
        const string rootNamesapce = "QueryPack.ModelManager.Models";

        public JsonSchemaMigrationProcessorImpl(ICompilationService compilationService,
            ISchemaParser schemaParser,
            IScaffoldService scaffoldService,
            PersistanceContext persistanceContext)
        {
            _compilationService = compilationService;
            _persistanceContext = persistanceContext;
            _schemaParser = schemaParser;
            _scaffoldService = scaffoldService;
        }

        public async Task<OneOf<Success, Failure>> ProcessAsync(string json)
        {
            var parsingResult = await _schemaParser.ParseAsync(json);
            if (!parsingResult.IsValid)
            {
                return new Failure(parsingResult.ValidationResult);
            }

            var schemaKey = parsingResult.SchemaKey;
            var lastSnapshot = await _persistanceContext.Snapshots.OrderBy(e => e.CreatedAt).LastOrDefaultAsync();
            var lastSchema = await _persistanceContext.Schemas.FirstOrDefaultAsync(e => e.ProductId == schemaKey.ProductId && e.IsCurrent);

            var codeGenOpts = new ModelCodeGenerationOptions()
            {
                RootNamespace = rootNamesapce,
                ContextName = scaffoldedContextClassName,
                ContextNamespace = rootNamesapce,
                ModelNamespace = rootNamesapce,
                UseDataAnnotations = true,
                SuppressConnectionStringWarning = true,
            };

            var scaffoldedModelSources = _scaffoldService.ScaffoldModel(codeGenOpts);
            var referencedAssemblies = _scaffoldService.GetType().Assembly.GetReferencedAssemblies()
                .Select(a => Assembly.Load(a));

            var translator = new CSharpSchemaTranslator(new TranslationOptions(parsingResult.SchemaResolver, rootNamesapce, scaffoldedModelSources.AdditionalFiles.Select(e => e.Code)));
            var translationResult = translator.Translate(parsingResult.JsonSchema);

            var sourceFiles = new Dictionary<string, IEnumerable<string>>
            {
                ["scaffoldedContextFile"] = new[] { scaffoldedModelSources.ContextFile.Code }
            };

            var schemaModelSources = translationResult.TranlsatedFiles;
            var contextGenerator = new CSharpContextGenerator();
            var compilesContextFile = contextGenerator.Generate(rootNamesapce, compiledContextClassName, scaffoldedContextClassName, schemaModelSources.Select(e => e.ClassName));

            sourceFiles["translatedFiles"] = schemaModelSources.Select(e => e.SourceCode);
            sourceFiles["scaffoldedFiles"] = translationResult.AdditionalFiles.Select(e => e.SourceCode);
            sourceFiles["compiledContextFile"] = new[] { compilesContextFile };

            var snapshot = lastSnapshot?.SnapShotCode;
            if (snapshot != null)
                sourceFiles["snapshotFile"] = new[] { snapshot };

            var dynamicContextAssembly = _compilationService.Compile(sourceFiles.Values.SelectMany(e => e), referencedAssemblies.ToArray());
            var dynamicContext = GetContext(dynamicContextAssembly, rootNamesapce, compiledContextClassName);

            var scaffoldedMigration = _scaffoldService.ScaffoldMigration(dynamicContext);
            sourceFiles["migrationFiles"] = new[] { scaffoldedMigration.MetadataCode, scaffoldedMigration.MigrationCode };
            sourceFiles["snapshotFile"] = new[] { scaffoldedMigration.SnapshotCode };

            var migrationAssembly = _compilationService.Compile(sourceFiles.Values.SelectMany(e => e), referencedAssemblies.ToArray());
            var migrationContext = GetContext(migrationAssembly, rootNamesapce, compiledContextClassName);

            if (lastSchema != null)
            {
                lastSchema.IsCurrent = false;
            }

            var migratedSchema = new Management.Persistance.Schema
            {
                JsonSchema = json,
                Version = parsingResult.SchemaKey.Version,
                ProductId = parsingResult.SchemaKey.ProductId,
                IsProvisioned = true,
                ProvisionedAt = DateTimeOffset.UtcNow,
                IsCurrent = true,
                IsApproved = true,
                ApprovedAt = DateTimeOffset.UtcNow,
                Snapshot = new Snapshot
                {
                    SnapShotCode = scaffoldedMigration.SnapshotCode,
                },
                Previous = lastSchema,
                MigrationCode = scaffoldedMigration.MigrationCode,
                MigrationMetaCode = scaffoldedMigration.MetadataCode,
                MigrationId = scaffoldedMigration.MigrationId
            };

            try
            {
                migrationContext.Database.Migrate();
                await _persistanceContext.Schemas.AddAsync(migratedSchema);
                await _persistanceContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errors = new List<string>();
                Exception exception = ex;

                while (exception != null)
                {
                    errors.Add(exception.Message);
                    exception = exception.InnerException;
                }

                return new Failure(new ValidationResult
                {
                    [scaffoldedMigration.MigrationId] = errors
                });
            }

            return new Success();
        }

        static DbContext GetContext(Assembly assembly, string rootNamesapce, string contextClassName)
        {
            var type = assembly.GetType($"{rootNamesapce}.{contextClassName}");
            _ = type ?? throw new Exception("DataContext type not found");

            var ctor = type.GetConstructor(Type.EmptyTypes);
            _ = ctor ?? throw new Exception("DataContext ctor not found");

            return (DbContext)ctor.Invoke(null);
        }
    }
}