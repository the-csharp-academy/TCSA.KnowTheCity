using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Models.Domain;

namespace TCSA.KnowTheCity.Tests.Helpers;

internal static class DbContextFactory
{
    internal static async Task<KnowTheCityDbContext> CreateAsync(
        DateTime? lastSync = null,
        string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<KnowTheCityDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var db = new KnowTheCityDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Configurations.Add(new Configurations
        {
            LastSync = lastSync ?? DateTime.UtcNow.AddDays(-2)
        });

        await db.SaveChangesAsync();
        return db;
    }
}