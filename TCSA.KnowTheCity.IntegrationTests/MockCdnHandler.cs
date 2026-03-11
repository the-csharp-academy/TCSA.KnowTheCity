using System.Net;
using System.Text;

namespace TCSA.KnowTheCity.IntegrationTests;

/// <summary>
/// An <see cref="HttpMessageHandler"/> that serves pre-defined JSON responses
/// keyed by absolute URL, simulating a CDN without any network traffic.
/// </summary>
internal sealed class MockCdnHandler(Dictionary<string, string> routes) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = request.RequestUri!.AbsoluteUri.TrimEnd('/');

        if (!routes.TryGetValue(url, out var json))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent($"No mock route registered for: {url}")
            });
        }

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}