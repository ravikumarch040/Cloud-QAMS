extern alias AuditInspectionApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class AuditInspectionApiIntegrationTests : IClassFixture<WebApplicationFactory<AuditInspectionApi::Program>>
{
    private readonly HttpClient _client;

    public AuditInspectionApiIntegrationTests(WebApplicationFactory<AuditInspectionApi::Program> factory)
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

        Assert.Equal("Qams.AuditInspection.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndCompleteInspection_ReturnsCompletedInspection()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            auditType = "Internal",
            scope = "Manufacturing area A",
            auditors = new[] { "auditor-1", "auditor-2" }
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/inspections")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-inspection-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var inspectionId = createDoc.RootElement.GetProperty("data").GetProperty("inspectionId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(inspectionId));

        var completeRequest = new
        {
            tenantId = "tenant-demo",
            findings = "No significant non-conformances found",
            riskRating = "Low"
        };

        var completeMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/inspections/{inspectionId}/complete")
        {
            Content = JsonContent.Create(completeRequest)
        };
        completeMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        completeMessage.Headers.Add("Idempotency-Key", "complete-inspection-001");

        var completeResponse = await _client.SendAsync(completeMessage);
        await EnsureSuccessStatusCodeWithContent(completeResponse);

        using var completeDoc = await JsonDocument.ParseAsync(await completeResponse.Content.ReadAsStreamAsync());
        var responseData = completeDoc.RootElement.GetProperty("data");
        Assert.Equal("Completed", responseData.GetProperty("status").GetString());
    }
}
