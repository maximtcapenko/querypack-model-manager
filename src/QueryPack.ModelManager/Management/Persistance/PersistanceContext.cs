namespace QueryPack.ModelManager.Management.Persistance
{
    using Microsoft.EntityFrameworkCore;

    public class PersistanceContext : DbContext
    {
        public PersistanceContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Schema> Schemas { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
    }
}