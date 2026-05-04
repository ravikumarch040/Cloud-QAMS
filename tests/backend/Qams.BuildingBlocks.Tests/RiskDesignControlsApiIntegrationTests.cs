extern alias RiskDesignControlsApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class RiskDesignControlsApiIntegrationTests : IClassFixture<WebApplicationFactory<RiskDesignControlsApi::Program>>
{
    private readonly HttpClient _client;

    public RiskDesignControlsApiIntegrationTests(WebApplicationFactory<RiskDesignControlsApi::Program> factory)
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

        Assert.Equal("Qams.RiskDesignControls.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateRiskAssessment_ReturnsCreatedAssessment()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            title = "Initial Risk Assessment",
            description = "Device design and manufacturing process risk assessment",
            riskLevel = "Medium"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/risk-assessments")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-risk-assessment-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("riskAssessmentId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("Initial Risk Assessment", data.GetProperty("title").GetString());
        Assert.Equal("Medium", data.GetProperty("riskLevel").GetString());
    }
}