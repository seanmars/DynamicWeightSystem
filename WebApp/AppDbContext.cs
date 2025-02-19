using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp;

public class AppDbContext : DbContext
{
    public DbSet<FishSampling> FishSamplings { get; set; }
    public DbSet<FishData> FishData { get; set; }
    public DbSet<WeightLevel> WeightLevels { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FishData>().ToTable("FishData");
        modelBuilder.Entity<FishData>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<FishData>()
            .Property(x => x.FishCode)
            .IsUnicode()
            .ValueGeneratedNever();


        modelBuilder.Entity<FishSampling>().ToTable("FishSamplings");
        modelBuilder.Entity<FishSampling>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<FishSampling>()
            .Property(x => x.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<WeightLevel>().ToTable("WeightLevel");
        modelBuilder.Entity<WeightLevel>()
            .HasKey(x => x.Level);
        modelBuilder.Entity<WeightLevel>()
            .Property(x => x.Level)
            .ValueGeneratedNever();
    }

    public async Task UpsertWeightLevel(List<WeightLevel> entities, CancellationToken ct)
    {
        foreach (var entity in entities)
        {
            var exist = await WeightLevels.FirstOrDefaultAsync(x => x.Level == entity.Level,
                cancellationToken: ct);

            if (exist == null)
            {
                WeightLevels.Add(entity);
            }
            else
            {
                Entry(exist).CurrentValues.SetValues(entity);
            }
        }

        await SaveChangesAsync(ct);
    }
}