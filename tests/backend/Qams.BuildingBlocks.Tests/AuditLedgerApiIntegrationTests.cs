extern alias AuditLedgerApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class AuditLedgerApiIntegrationTests : IClassFixture<WebApplicationFactory<AuditLedgerApi::Program>>
{
    private readonly HttpClient _client;

    public AuditLedgerApiIntegrationTests(WebApplicationFactory<AuditLedgerApi::Program> factory)
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

        Assert.Equal("Qams.AuditLedger.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task AppendAuditEntry_ReturnsCreatedEntry()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            actorId = "user-123",
            action = "Created CAPA case",
            recordType = "CapaCase",
            recordId = "capa-001",
            recordVersion = "1",
            reason = "Initial CAPA case creation for supplier quality issue"
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/audit-ledger")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "audit-entry-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("auditEntryId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("user-123", data.GetProperty("actorId").GetString());
        Assert.Equal("Created CAPA case", data.GetProperty("action").GetString());
        Assert.Equal("CapaCase", data.GetProperty("recordType").GetString());
        Assert.Equal("capa-001", data.GetProperty("recordId").GetString());
    }
}