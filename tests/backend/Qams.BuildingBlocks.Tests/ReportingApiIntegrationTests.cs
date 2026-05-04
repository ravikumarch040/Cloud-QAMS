extern alias ReportingApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ReportingApiIntegrationTests : IClassFixture<WebApplicationFactory<ReportingApi::Program>>
{
    private readonly HttpClient _client;

    public ReportingApiIntegrationTests(WebApplicationFactory<ReportingApi::Program> factory)
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

        Assert.Equal("Qams.Reporting.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ListReportDefinitions_ReturnsSeedDefinitions()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/report-definitions");
        request.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");
        request.Headers.Add("X-Correlation-Id", "test-corr-001");

        var response = await _client.SendAsync(request);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() > 0);

        var firstDef = data[0];
        Assert.NotNull(firstDef.GetProperty("reportDefinitionId").GetString());
        Assert.NotNull(firstDef.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GenerateReport_ReturnsGeneratedReport()
    {
        // First, list available report definitions
        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/report-definitions");
        listRequest.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");
        var listResponse = await _client.SendAsync(listRequest);
        await EnsureSuccessStatusCodeWithContent(listResponse);

        using var listDoc = await JsonDocument.ParseAsync(await listResponse.Content.ReadAsStreamAsync());
        var definitions = listDoc.RootElement.GetProperty("data");
        var firstDefId = definitions[0].GetProperty("reportDefinitionId").GetString();

        // Now generate a report for the first definition
        var generateRequest = new
        {
            tenantId = "tenant-life-sciences-demo",
            reportDefinitionId = firstDefId,
            filters = new Dictionary<string, string> { ["status"] = "Open" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/reports")
        {
            Content = JsonContent.Create(generateRequest)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");
        message.Headers.Add("Idempotency-Key", "generate-report-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.NotNull(data.GetProperty("reportId").GetString());
        Assert.Equal(firstDefId, data.GetProperty("reportDefinitionId").GetString());
    }

    [Fact]
    public async Task GenerateReport_FailsForNonexistentDefinition()
    {
        var generateRequest = new
        {
            tenantId = "tenant-demo",
            reportDefinitionId = "nonexistent-report-def",
            filters = new Dictionary<string, string>()
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/reports")
        {
            Content = JsonContent.Create(generateRequest)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "generate-report-bad-001");

        var response = await _client.SendAsync(message);
        Assert.False(response.IsSuccessStatusCode);
    }
}
