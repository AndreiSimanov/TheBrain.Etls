using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheBrain.Etls.Models;

namespace TheBrain.Etls.DBContext;

internal class SqliteContext : DbContext
{
    public DbSet<Thought> Thoughts { get; set; }
    public DbSet<SyncPoint> SyncPoints { get; set; }

    private static int crmContextId;
    public int Id { get; set; }
    string dbPath = string.Empty;

    public SqliteContext(string dbPath)
    {
        SQLitePCL.Batteries_V2.Init();
        crmContextId++;
        Id = crmContextId;
        this.dbPath = dbPath;
        // QueryTrackingBehavior should be Tracking to update thought names
        //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Filename={dbPath}");
        //optionsBuilder.LogTo(message => EtlLog.Information(message, false));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<SyncPoint>().ToTable("SyncPoints");
        modelBuilder.Entity<SyncPoint>().Property(e => e.CreationDateTime).HasConversion<DateTimeToTicksConverter>();
        modelBuilder.Entity<Thought>().ToTable("Thoughts");
        modelBuilder.Entity<Thought>().Property(e => e.ModificationDateTime).HasConversion<DateTimeToTicksConverter>();
        modelBuilder.Entity<Thought>().Property(e => e.SyncUpdateDateTime).HasConversion<long?>();
        modelBuilder.Entity<Thought>().Ignore(t => t.ContentPath);
    }
}

