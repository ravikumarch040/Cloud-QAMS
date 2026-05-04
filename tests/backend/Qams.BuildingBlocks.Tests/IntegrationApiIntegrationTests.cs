extern alias IntegrationApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class IntegrationApiIntegrationTests : IClassFixture<WebApplicationFactory<IntegrationApi::Program>>
{
    private readonly HttpClient _client;

    public IntegrationApiIntegrationTests(WebApplicationFactory<IntegrationApi::Program> factory)
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

        Assert.Equal("Qams.Integration.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndTestConnector_ReturnsTestResult()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            name = "ERP System Connector",
            connectorType = "REST",
            endpointUrl = "https://api.erp-system.com/v1",
            authenticationType = "BearerToken",
            configuration = new { apiKey = "test-key-123" }
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/connectors")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-connector-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var connectorId = createDoc.RootElement.GetProperty("data").GetProperty("connectorId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(connectorId));

        var testRequest = new
        {
            tenantId = "tenant-demo"
        };

        var testMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/connectors/{connectorId}/test")
        {
            Content = JsonContent.Create(testRequest)
        };
        testMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        testMessage.Headers.Add("Idempotency-Key", "test-connector-001");

        var testResponse = await _client.SendAsync(testMessage);
        await EnsureSuccessStatusCodeWithContent(testResponse);

        using var testDoc = await JsonDocument.ParseAsync(await testResponse.Content.ReadAsStreamAsync());
        var responseData = testDoc.RootElement.GetProperty("data");
        Assert.True(responseData.GetProperty("connected").GetBoolean());
    }
}