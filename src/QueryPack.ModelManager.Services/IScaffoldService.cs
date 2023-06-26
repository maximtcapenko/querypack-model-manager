namespace QueryPack.ModelManager.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations.Design;
    using Microsoft.EntityFrameworkCore.Scaffolding;

    public interface IScaffoldService
    {
        ScaffoldedModel ScaffoldModel(ModelCodeGenerationOptions options);
        ScaffoldedMigration ScaffoldMigration(DbContext context);
    }
}