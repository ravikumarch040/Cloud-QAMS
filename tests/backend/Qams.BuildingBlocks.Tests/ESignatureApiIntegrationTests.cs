extern alias ESignatureApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ESignatureApiIntegrationTests : IClassFixture<WebApplicationFactory<ESignatureApi::Program>>
{
    private readonly HttpClient _client;

    public ESignatureApiIntegrationTests(WebApplicationFactory<ESignatureApi::Program> factory)
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

        Assert.Equal("Qams.ESignature.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateESignature_ReturnsCreatedSignature()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            recordType = "DocumentApproval",
            recordId = "doc-001",
            recordVersion = "1",
            signedMeaning = "Approve quality procedure",
            reason = "Document review completed",
            signerUserId = "user-123",
            signerDisplayName = "John Doe",
            reauthMethod = "Password",
            reauthReference = "auth-123",
            recordHash = "abc123def456",
            previousAuditHash = (string?)null
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/esignatures")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "esignature-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("signatureId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("user-123", data.GetProperty("signerUserId").GetString());
        Assert.Equal("DocumentApproval", data.GetProperty("recordType").GetString());
        Assert.Equal("doc-001", data.GetProperty("recordId").GetString());
    }
}