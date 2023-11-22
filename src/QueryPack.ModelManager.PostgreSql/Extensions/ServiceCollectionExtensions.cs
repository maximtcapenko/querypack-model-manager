namespace QueryPack.ModelManager.PostgreSql.Extensions
{
    using Services;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseSqlServerSchemaServices(this IServiceCollection self,
            Action<PostgreSqlServicesOptions> configurer)
        {
            var options = new PostgreSqlServicesOptions();
            configurer?.Invoke(options);

            // validate options instance
            if (string.IsNullOrEmpty(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString));

            self.AddSingleton(options);
            self.AddSingleton<IScaffoldService, PostgreSqlScaffoldServiceImpl>();
            return self;
        }
    }
}
