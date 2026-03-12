using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Core.Data;

public class KnowTheCityDbContext(DbContextOptions<KnowTheCityDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<Landmark> Landmarks => Set<Landmark>();
    public DbSet<GameResult> GameResults => Set<GameResult>();
    public DbSet<GameResultItem> GameResultItems => Set<GameResultItem>();
    public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();
    public DbSet<FavoriteLandmark> FavoriteLandmarks => Set<FavoriteLandmark>();
    public DbSet<Configurations> Configurations => Set<Configurations>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired();
            entity.HasMany(c => c.Landmarks)
                  .WithOne(l => l.City)
                  .HasForeignKey(l => l.CityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Landmark>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired();
        });

        modelBuilder.Entity<FavoriteCity>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => f.CityId).IsUnique();
        });

        modelBuilder.Entity<FavoriteLandmark>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => new { f.CityId, f.LandmarkId }).IsUnique();
        });
    }
}