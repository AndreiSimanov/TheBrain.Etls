using Microsoft.EntityFrameworkCore;
using TheBrain.Etls.Models;

namespace TheBrain.Etls.DBContext;

internal class SqliteContext : DbContext
{
    public DbSet<Thoughts> Thoughts { get; set; }

    private static int crmContextId;
    public int Id { get; set; }
    string dbPath =string.Empty;

    public SqliteContext(string dbPath)
    {
        SQLitePCL.Batteries_V2.Init();
        crmContextId++;
        Id = crmContextId;
        this.dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Filename={dbPath}");
        base.OnConfiguring(optionsBuilder);
    }
}

