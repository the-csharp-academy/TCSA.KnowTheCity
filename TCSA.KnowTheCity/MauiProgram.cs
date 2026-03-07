using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TCSA.KnowTheCity.Data;
using TCSA.KnowTheCity.Services;

namespace TCSA.KnowTheCity
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "knowthecity.db");
            builder.Services.AddDbContextFactory<KnowTheCityDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddScoped<IGameService, GameService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<KnowTheCityDbContext>>()
                .CreateDbContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            return app;
        }
    }
}
