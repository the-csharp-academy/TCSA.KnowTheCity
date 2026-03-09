using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Models;

namespace TCSA.KnowTheCity.Data;

public class KnowTheCityDbContext(DbContextOptions<KnowTheCityDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> GameResults => Set<GameResult>();
    public DbSet<GameResultItem> GameResultItems => Set<GameResultItem>();
    public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();
    public DbSet<FavoriteLandmark> FavoriteLandmarks => Set<FavoriteLandmark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FavoriteCity>(entity =>
        {
            entity.Property(f => f.CityId).IsRequired();
            entity.HasIndex(f => f.CityId).IsUnique();
        });

        modelBuilder.Entity<FavoriteLandmark>(entity =>
        {
            entity.Property(f => f.CityId).IsRequired();
            entity.Property(f => f.LandmarkId).IsRequired();
            entity.HasIndex(f => new { f.CityId, f.LandmarkId }).IsUnique();
        });
    }
}