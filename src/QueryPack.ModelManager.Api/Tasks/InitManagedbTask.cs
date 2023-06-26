namespace QueryPack.ModelManager.Api.Tasks
{
    using Management.Persistance;

    internal class InitManagedbTask : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InitManagedbTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<PersistanceContext>();
            await context.Database.EnsureCreatedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
