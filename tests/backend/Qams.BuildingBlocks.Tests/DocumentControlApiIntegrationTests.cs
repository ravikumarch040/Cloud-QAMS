extern alias DocumentControlApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class DocumentControlApiIntegrationTests : IClassFixture<WebApplicationFactory<DocumentControlApi::Program>>
{
    private readonly HttpClient _client;

    public DocumentControlApiIntegrationTests(WebApplicationFactory<DocumentControlApi::Program> factory)
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

        Assert.Equal("Qams.DocumentControl.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndApproveDocument_ReturnsApprovedDocument()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            documentName = "Quality Procedure v1.0",
            category = "Procedures",
            status = "Draft"
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/documents")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-doc-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var documentId = createDoc.RootElement.GetProperty("data").GetProperty("documentId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(documentId));

        var approveRequest = new
        {
            tenantId = "tenant-demo",
            approverComments = "Approved for distribution",
            effectiveDate = "2026-05-04"
        };

        var approveMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/documents/{documentId}/approve")
        {
            Content = JsonContent.Create(approveRequest)
        };
        approveMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        approveMessage.Headers.Add("Idempotency-Key", "approve-doc-001");

        var approveResponse = await _client.SendAsync(approveMessage);
        await EnsureSuccessStatusCodeWithContent(approveResponse);

        using var approveDoc = await JsonDocument.ParseAsync(await approveResponse.Content.ReadAsStreamAsync());
        var responseData = approveDoc.RootElement.GetProperty("data");
        Assert.Equal("Approved", responseData.GetProperty("status").GetString());
    }
}
