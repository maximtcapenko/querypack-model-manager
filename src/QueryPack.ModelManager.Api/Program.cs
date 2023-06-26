namespace QueryPack.ModelManager.Api
{
    using Tasks;
    using Extensions;
    using Management.Persistance;
    using Infrastructure;
    using SqlServer.Extensions;
    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var managedb = builder.Configuration["managedb"];
            var db = builder.Configuration["db"];

            builder.Services.AddSwaggerGen()
                .AddSchemaProcessingExtensions()
                .UseSqlServerSchemaServices(options =>
                {
                    options.ConnectionString = db;
                }).AddDbContext<PersistanceContext>(options => options.UseSqlServer(managedb));

            builder.Services.AddHostedService<InitManagedbTask>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}