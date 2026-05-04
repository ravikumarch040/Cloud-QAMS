extern alias IdentityApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class IdentityApiIntegrationTests : IClassFixture<WebApplicationFactory<IdentityApi::Program>>
{
    private readonly HttpClient _client;

    public IdentityApiIntegrationTests(WebApplicationFactory<IdentityApi::Program> factory)
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

        Assert.Equal("Qams.Identity.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateTenant_ReturnsCreatedTenant()
    {
        var request = new
        {
            slug = "test-tenant",
            displayName = "Test Tenant Organization",
            validationTier = "Standard"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/tenants")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-tenant-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.Equal("tenant-test-tenant", data.GetProperty("tenantId").GetString());
        Assert.Equal("Test Tenant Organization", data.GetProperty("displayName").GetString());
        Assert.Equal("Standard", data.GetProperty("validationTier").GetString());
        Assert.Equal("Provisioning", data.GetProperty("status").GetString());
    }
}