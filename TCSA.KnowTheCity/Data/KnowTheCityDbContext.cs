using Microsoft.EntityFrameworkCore;
using TCSA.KnowTheCity.Models;

namespace TCSA.KnowTheCity.Data;

public class KnowTheCityDbContext(DbContextOptions<KnowTheCityDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> GameResults => Set<GameResult>();
    public DbSet<GameResultItem> GameResultItems => Set<GameResultItem>();
}