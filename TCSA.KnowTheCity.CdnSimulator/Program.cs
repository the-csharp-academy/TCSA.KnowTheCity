using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CdnOptions>(builder.Configuration.GetSection(CdnOptions.SectionName));

var app = builder.Build();

var contentRoot = app.Environment.ContentRootPath;
var webRoot = string.IsNullOrWhiteSpace(app.Environment.WebRootPath)
    ? Path.Combine(contentRoot, "wwwroot")
    : app.Environment.WebRootPath;

var manifestsRoot = Path.Combine(webRoot, "manifests");
var citiesRoot = Path.Combine(webRoot, "cities");
var landmarksRoot = Path.Combine(webRoot, "landmarks");
var flagsRoot = Path.Combine(webRoot, "flags");

Directory.CreateDirectory(citiesRoot);
Directory.CreateDirectory(landmarksRoot);
Directory.CreateDirectory(flagsRoot);

var contentTypeProvider = CreateContentTypeProvider();

app.UseHttpsRedirection();

UseStaticFolder(app, "/manifests", manifestsRoot, contentTypeProvider);
UseStaticFolder(app, "/cities", citiesRoot, contentTypeProvider);
UseStaticFolder(app, "/landmarks", landmarksRoot, contentTypeProvider);
UseStaticFolder(app, "/flags", flagsRoot, contentTypeProvider);

app.MapGet("/", (HttpContext httpContext, IOptions<CdnOptions> options) =>
{
    var baseUrl = ResolveBaseUrl(httpContext, options.Value);

    var manifestFiles = Directory.EnumerateFiles(manifestsRoot, "*.json", SearchOption.TopDirectoryOnly)
        .Select(Path.GetFileName)
        .Where(static name => !string.IsNullOrWhiteSpace(name))
        .OrderBy(static name => name, StringComparer.OrdinalIgnoreCase)
        .Select(name => $"{baseUrl}/manifests/{name}")
        .ToArray();

    return Results.Ok(new
    {
        service = "TCSA.KnowTheCity CDN Simulator",
        baseUrl,
        endpoints = new
        {
            health = $"{baseUrl}/health",
            cityCatalog = $"{baseUrl}/manifests/cities.json",
            manifests = manifestFiles,
            cities = $"{baseUrl}/cities",
            landmarks = $"{baseUrl}/landmarks",
            flags = $"{baseUrl}/flags"
        }
    });
});

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    utc = DateTimeOffset.UtcNow
}));


app.Run();

static void UseStaticFolder(
    WebApplication app,
    string requestPath,
    string physicalPath,
    FileExtensionContentTypeProvider contentTypeProvider)
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(physicalPath),
        RequestPath = requestPath,
        ContentTypeProvider = contentTypeProvider,
        OnPrepareResponse = context =>
        {
            var options = context.Context.RequestServices
                .GetRequiredService<IOptions<CdnOptions>>()
                .Value;

            context.Context.Response.Headers.CacheControl = $"public,max-age={options.CacheSeconds}";
        }
    });
}

static FileExtensionContentTypeProvider CreateContentTypeProvider()
{
    var provider = new FileExtensionContentTypeProvider();

    provider.Mappings[".json"] = "application/json";
    provider.Mappings[".webp"] = "image/webp";
    provider.Mappings[".svg"] = "image/svg+xml";
    provider.Mappings[".png"] = "image/png";
    provider.Mappings[".jpg"] = "image/jpeg";
    provider.Mappings[".jpeg"] = "image/jpeg";

    return provider;
}

static string ResolveBaseUrl(HttpContext httpContext, CdnOptions options)
{
    if (!string.IsNullOrWhiteSpace(options.PublicBaseUrl))
    {
        return options.PublicBaseUrl.TrimEnd('/');
    }

    return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}".TrimEnd('/');
}

internal sealed class CdnOptions
{
    public const string SectionName = "Cdn";

    public string? PublicBaseUrl { get; set; }

    public int CacheSeconds { get; set; } = 300;
}
