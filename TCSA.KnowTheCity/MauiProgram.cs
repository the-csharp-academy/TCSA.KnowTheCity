using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TCSA.KnowTheCity.Core.Clients;
using TCSA.KnowTheCity.Core.Data;
using TCSA.KnowTheCity.Core.Helpers;
using TCSA.KnowTheCity.Core.Options;
using TCSA.KnowTheCity.Core.Services;
using TCSA.KnowTheCity.Localization;
using TCSA.KnowTheCity.Services;

namespace TCSA.KnowTheCity;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter()
                .GetResult();

            builder.Configuration.AddJsonStream(stream);
        }
        catch (Exception)
        {
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Config:CdnUrl"] = "http://cdn.knowthecitygame.com"
            });
        }

        builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection("Config"));
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Localization");
        builder.Services.AddSingleton<AppLocalizationService>();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "knowthecity.db");
        builder.Services.AddDbContextFactory<KnowTheCityDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IFavoriteService, FavoriteService>();
        builder.Services.AddScoped<ICityService, CityService>();
        builder.Services.AddScoped<ISyncService, SyncService>();
        builder.Services.AddSingleton<IImageCacheService, ImageCacheService>();

        builder.Services.AddHttpClient<IManifestClient, ManifestClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        builder.Services.AddHttpClient<ImageCacheService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddSingleton<IImageCacheService>(sp =>
            sp.GetRequiredService<ImageCacheService>());

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        app.Services.GetRequiredService<AppLocalizationService>().Initialize();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<KnowTheCityDbContext>>()
            .CreateDbContext();

        //db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        //SeedCatalog(db);

        return app;
    }

    private static void SeedCatalog(KnowTheCityDbContext db)
    {
        if (db.Cities.Any())
            return;

        db.Configurations.Add(new() { LastSync = new DateTime(2026, 1, 1) });
        db.Cities.AddRange(CityDataHelper.SeedData);
        db.SaveChanges();
    }
}
