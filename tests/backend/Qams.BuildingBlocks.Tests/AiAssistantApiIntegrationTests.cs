extern alias AiAssistantApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class AiAssistantApiIntegrationTests : IClassFixture<WebApplicationFactory<AiAssistantApi::Program>>
{
    private readonly HttpClient _client;

    public AiAssistantApiIntegrationTests(WebApplicationFactory<AiAssistantApi::Program> factory)
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

        Assert.Equal("Qams.AiAssistant.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAssistantRequest_ReturnsResponse()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            userId = "user-123",
            promptText = "Analyze this quality issue and suggest corrective actions"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/assistant/requests")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "assistant-req-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("responseId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("user-123", data.GetProperty("userId").GetString());
        Assert.Equal("Analyze this quality issue and suggest corrective actions", data.GetProperty("promptText").GetString());
        Assert.Contains("Assistant response", data.GetProperty("responseText").GetString());
    }
}