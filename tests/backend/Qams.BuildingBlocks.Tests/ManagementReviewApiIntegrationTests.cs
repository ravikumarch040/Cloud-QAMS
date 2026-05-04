extern alias ManagementReviewApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ManagementReviewApiIntegrationTests : IClassFixture<WebApplicationFactory<ManagementReviewApi::Program>>
{
    private readonly HttpClient _client;

    public ManagementReviewApiIntegrationTests(WebApplicationFactory<ManagementReviewApi::Program> factory)
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

        Assert.Equal("Qams.ManagementReview.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateReviewPack_ReturnsCreatedPack()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            title = "Q1-2026 Quality System Review",
            summary = "Quarterly review of all quality processes and systems",
            reviewDateUtc = DateTimeOffset.UtcNow.AddDays(30),
            actionItems = new[] { "Assess system effectiveness", "Identify improvement opportunities" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/review-packs")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-review-pack-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("reviewPackId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("Q1-2026 Quality System Review", data.GetProperty("title").GetString());
        Assert.Equal("Quarterly review of all quality processes and systems", data.GetProperty("summary").GetString());
    }
}