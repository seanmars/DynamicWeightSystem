using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp;

public class AppDbContext : DbContext
{
    public DbSet<FishSampling> FishSamplings { get; set; }
    public DbSet<FishData> FishData { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FishData>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<FishData>()
            .Property(x => x.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<FishSampling>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<FishSampling>()
            .Property(x => x.Id)
            .ValueGeneratedNever();
    }
}