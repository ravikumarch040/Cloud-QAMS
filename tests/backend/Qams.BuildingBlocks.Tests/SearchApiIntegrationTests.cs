extern alias SearchApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class SearchApiIntegrationTests : IClassFixture<WebApplicationFactory<SearchApi::Program>>
{
    private readonly HttpClient _client;

    public SearchApiIntegrationTests(WebApplicationFactory<SearchApi::Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static async Task EnsureSuccessStatusCodeWithContent(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status {(int)response.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var root = doc.RootElement;

        Assert.Equal("Qams.Search.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Search_ReturnsResultsForTenantQuery()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/search?query=Supplier");
        request.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(request);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() > 0);
        Assert.Contains(data.EnumerateArray(), item => item.GetProperty("title").GetString()?.Contains("Supplier", StringComparison.OrdinalIgnoreCase) == true);
    }

    [Fact]
    public async Task Search_EmptyQuery_ReturnsNoResults()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/search?query=");
        request.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(request);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.Equal(0, data.GetArrayLength());
    }
}
