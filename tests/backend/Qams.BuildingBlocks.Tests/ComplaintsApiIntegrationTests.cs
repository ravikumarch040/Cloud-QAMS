extern alias ComplaintsApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ComplaintsApiIntegrationTests : IClassFixture<WebApplicationFactory<ComplaintsApi::Program>>
{
    private readonly HttpClient _client;

    public ComplaintsApiIntegrationTests(WebApplicationFactory<ComplaintsApi::Program> factory)
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

        Assert.Equal("Qams.Complaints.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndResolveComplaint_ReturnsResolvedComplaint()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            complaintSource = "Customer",
            description = "Product quality issue reported",
            severity = "High",
            productBatchId = "batch-12345"
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/complaints")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-complaint-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var complaintId = createDoc.RootElement.GetProperty("data").GetProperty("complaintId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(complaintId));

        var resolveRequest = new
        {
            tenantId = "tenant-demo",
            resolutionSummary = "Root cause identified and corrected",
            closureDate = "2026-05-10"
        };

        var resolveMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/complaints/{complaintId}/resolve")
        {
            Content = JsonContent.Create(resolveRequest)
        };
        resolveMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        resolveMessage.Headers.Add("Idempotency-Key", "resolve-complaint-001");

        var resolveResponse = await _client.SendAsync(resolveMessage);
        await EnsureSuccessStatusCodeWithContent(resolveResponse);

        using var resolveDoc = await JsonDocument.ParseAsync(await resolveResponse.Content.ReadAsStreamAsync());
        var responseData = resolveDoc.RootElement.GetProperty("data");
        Assert.Equal("Resolved", responseData.GetProperty("status").GetString());
    }
}
