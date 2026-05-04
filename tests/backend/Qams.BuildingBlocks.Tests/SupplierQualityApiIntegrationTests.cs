extern alias SupplierQualityApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class SupplierQualityApiIntegrationTests : IClassFixture<WebApplicationFactory<SupplierQualityApi::Program>>
{
    private readonly HttpClient _client;

    public SupplierQualityApiIntegrationTests(WebApplicationFactory<SupplierQualityApi::Program> factory)
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

        Assert.Equal("Qams.SupplierQuality.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndUpdateSupplierScore_ReturnsUpdatedSupplier()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            name = "Medical Components Inc",
            score = 90,
            status = "Approved",
            lastAuditAtUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/suppliers")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-supplier-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var supplierId = createDoc.RootElement.GetProperty("data").GetProperty("supplierId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(supplierId));

        var updateRequest = new
        {
            tenantId = "tenant-demo",
            score = 85,
            status = "Approved",
            lastAuditAtUtc = DateTimeOffset.Parse("2026-04-01T00:00:00Z")
        };

        var updateMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/suppliers/{supplierId}/score")
        {
            Content = JsonContent.Create(updateRequest)
        };
        updateMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        updateMessage.Headers.Add("Idempotency-Key", "update-supplier-score-001");

        var updateResponse = await _client.SendAsync(updateMessage);
        await EnsureSuccessStatusCodeWithContent(updateResponse);

        using var updateDoc = await JsonDocument.ParseAsync(await updateResponse.Content.ReadAsStreamAsync());
        var responseData = updateDoc.RootElement.GetProperty("data");
        Assert.Equal("Approved", responseData.GetProperty("status").GetString());
    }
}