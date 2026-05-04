extern alias QualityEventsApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class QualityEventsApiIntegrationTests : IClassFixture<WebApplicationFactory<QualityEventsApi::Program>>
{
    private readonly HttpClient _client;

    public QualityEventsApiIntegrationTests(WebApplicationFactory<QualityEventsApi::Program> factory)
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

        Assert.Equal("Qams.QualityEvents.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ListQualityEvents_ReturnsSeedEvents()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/quality-events");
        request.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");
        request.Headers.Add("X-Correlation-Id", "test-corr-001");

        var response = await _client.SendAsync(request);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() > 0);

        var firstEvent = data[0];
        Assert.Equal("Temperature excursion in controlled storage", firstEvent.GetProperty("title").GetString());
    }

    [Fact]
    public async Task CreateQualityEvent_ReturnsCreatedEvent()
    {
        var createRequest = new
        {
            eventType = "Deviation",
            title = "Failed batch sterility test",
            severity = "Critical"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/quality-events")
        {
            Content = JsonContent.Create(createRequest)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-qe-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.NotNull(data.GetProperty("qualityEventId").GetString());
        Assert.Equal("Failed batch sterility test", data.GetProperty("title").GetString());
        Assert.Equal("Open", data.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateQualityEvent_RequiresTenantHeader()
    {
        var createRequest = new
        {
            eventType = "Deviation",
            title = "Test event",
            severity = "High"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/quality-events")
        {
            Content = JsonContent.Create(createRequest)
        };
        // Missing X-Tenant-Id
        message.Headers.Add("Idempotency-Key", "create-qe-002");

        var response = await _client.SendAsync(message);
        Assert.False(response.IsSuccessStatusCode);
    }
}
