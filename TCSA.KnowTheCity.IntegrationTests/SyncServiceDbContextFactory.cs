using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TCSA.KnowTheCity.Core.Data;

namespace TCSA.KnowTheCity.IntegrationTests;

/// <summary>
/// Returns a <see cref="NonDisposingDbContextWrapper"/> each time so that
/// <c>await using var db = ...</c> inside <see cref="SyncService"/> disposes
/// only the wrapper, leaving the real context alive for test assertions.
/// </summary>
internal sealed class SyncServiceDbContextFactory(KnowTheCityDbContext db)
    : IDbContextFactory<KnowTheCityDbContext>
{
    public KnowTheCityDbContext CreateDbContext() => new NonDisposingDbContextWrapper(db);

    public Task<KnowTheCityDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<KnowTheCityDbContext>(new NonDisposingDbContextWrapper(db));
}

/// <summary>
/// Delegates every operation to the inner context but swallows
/// <see cref="Dispose"/> / <see cref="DisposeAsync"/> so the real context
/// is never torn down by the SUT.
/// </summary>
internal sealed class NonDisposingDbContextWrapper(KnowTheCityDbContext inner) : KnowTheCityDbContext(
    new DbContextOptionsBuilder<KnowTheCityDbContext>()
        .UseInMemoryDatabase("__unused__")
        .Options)
{
    // Route all DbSet access to the real context
    public override DbSet<T> Set<T>() => inner.Set<T>();

    // Route SaveChanges to the real context
    public override int SaveChanges() => inner.SaveChanges();
    public override int SaveChanges(bool acceptAllChangesOnSuccess) => inner.SaveChanges(acceptAllChangesOnSuccess);
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => inner.SaveChangesAsync(cancellationToken);
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) => inner.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

    // Swallow disposal — the test owns the real context's lifetime
    public override void Dispose() { }
    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
}