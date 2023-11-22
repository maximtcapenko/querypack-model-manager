namespace QueryPack.ModelManager.PostgreSql
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Migrations.Design;
    using Microsoft.EntityFrameworkCore.Scaffolding;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Design.Internal;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Diagnostics.Internal;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Scaffolding.Internal;

    internal class PostgreSqlScaffoldServiceImpl : IScaffoldService
    {
        private readonly PostgreSqlServicesOptions _options;

        public PostgreSqlScaffoldServiceImpl(PostgreSqlServicesOptions options)
        {
            _options = options;
        }

        public ScaffoldedMigration ScaffoldMigration(DbContext context)
        {
            var services = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices()
            .AddDbContextDesignTimeServices(context);

            var designTimeServices = new NpgsqlDesignTimeServices();
            designTimeServices.ConfigureDesignTimeServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var contextNameSpace = context.GetType().Namespace;

            var scaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
            return scaffolder.ScaffoldMigration(ResolveMigrationName(context), contextNameSpace);
        }

        public ScaffoldedModel ScaffoldModel(ModelCodeGenerationOptions options)
        {
            var scaffolder = Create();
            var dbModelFactoryOptions = new DatabaseModelFactoryOptions();
            var modelReverseEngineerOptions = new ModelReverseEngineerOptions();

            return scaffolder.ScaffoldModel(_options.ConnectionString!, dbModelFactoryOptions, modelReverseEngineerOptions, options);
        }

        static IReverseEngineerScaffolder Create() =>
        new ServiceCollection()
            .AddEntityFrameworkNpgsql()
            .AddLogging()
            .AddEntityFrameworkDesignTimeServices()
            .AddSingleton<LoggingDefinitions, NpgsqlLoggingDefinitions>()
            .AddSingleton<IRelationalTypeMappingSource, NpgsqlTypeMappingSource>()
            .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
            .AddSingleton<IDatabaseModelFactory, NpgsqlDatabaseModelFactory>()
            .AddSingleton<IProviderConfigurationCodeGenerator, NpgsqlCodeGenerator>()
            .AddSingleton<IScaffoldingModelFactory, RelationalScaffoldingModelFactory>()
            .AddSingleton<IPluralizer, Bricelam.EntityFrameworkCore.Design.Pluralizer>()
            .AddSingleton<ProviderCodeGeneratorDependencies>()
            .AddSingleton<AnnotationCodeGeneratorDependencies>()
            .BuildServiceProvider()
            .GetRequiredService<IReverseEngineerScaffolder>();

        // issue: add correct name resolver
        private static string ResolveMigrationName(DbContext context)
        {
            return $"Modify_{context.GetType().Name}";
        }
    }
}