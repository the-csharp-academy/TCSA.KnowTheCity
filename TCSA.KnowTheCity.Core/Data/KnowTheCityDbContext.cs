using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Core.Data;

public class KnowTheCityDbContext(DbContextOptions<KnowTheCityDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<CityTranslation> CityTranslations => Set<CityTranslation>();
    public DbSet<Landmark> Landmarks => Set<Landmark>();
    public DbSet<LandmarkTranslation> LandmarkTranslations => Set<LandmarkTranslation>();
    public DbSet<GameResult> GameResults => Set<GameResult>();
    public DbSet<GameResultItem> GameResultItems => Set<GameResultItem>();
    public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();
    public DbSet<FavoriteLandmark> FavoriteLandmarks => Set<FavoriteLandmark>();
    public DbSet<Configurations> Configurations => Set<Configurations>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Configurations>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.LastSync).IsRequired();
            entity.HasData(new Configurations { Id = 1, LastSync = new DateTime(2026, 1, 1) });
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.ImagePath).IsRequired();
            entity.HasIndex(c => c.RemoteId).IsUnique();
            entity.HasMany(c => c.Landmarks)
                  .WithOne(l => l.City)
                  .HasForeignKey(l => l.CityId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Translations)
                .WithOne(t => t.City)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Landmark>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired();
            entity.Property(l => l.ImagePath).IsRequired();

            entity.HasMany(l => l.Translations)
                .WithOne(t => t.Landmark)
                .HasForeignKey(t => t.LandmarkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CityTranslation>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.LanguageCode).IsRequired();
            entity.Property(t => t.Name).IsRequired();
            entity.HasIndex(t => new { t.CityId, t.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<LandmarkTranslation>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.LanguageCode).IsRequired();
            entity.Property(t => t.Name).IsRequired();
            entity.HasIndex(t => new { t.LandmarkId, t.LanguageCode }).IsUnique();
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