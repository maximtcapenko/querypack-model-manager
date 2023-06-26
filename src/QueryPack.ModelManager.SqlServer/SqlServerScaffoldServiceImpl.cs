namespace QueryPack.ModelManager.SqlServer
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Migrations.Design;
    using Microsoft.EntityFrameworkCore.Scaffolding;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Diagnostics.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Services;


    internal class SqlServerScaffoldServiceImpl : IScaffoldService
    {
        private readonly SqlServerServicesOptions _options;

        public SqlServerScaffoldServiceImpl(SqlServerServicesOptions options)
        {
            _options = options;
        }

        public ScaffoldedMigration ScaffoldMigration(DbContext context)
        {
            var services = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices()
            .AddDbContextDesignTimeServices(context);

            var designTimeServices = new SqlServerDesignTimeServices();
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

            return scaffolder.ScaffoldModel(_options.ConnectionString, dbModelFactoryOptions, modelReverseEngineerOptions, options);
        }

        static IReverseEngineerScaffolder Create() =>
        new ServiceCollection()
            .AddEntityFrameworkSqlServer()
            .AddLogging()
            .AddEntityFrameworkDesignTimeServices()
            .AddSingleton<LoggingDefinitions, SqlServerLoggingDefinitions>()
            .AddSingleton<IRelationalTypeMappingSource, SqlServerTypeMappingSource>()
            .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
            .AddSingleton<IDatabaseModelFactory, SqlServerDatabaseModelFactory>()
            .AddSingleton<IProviderConfigurationCodeGenerator, SqlServerCodeGenerator>()
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