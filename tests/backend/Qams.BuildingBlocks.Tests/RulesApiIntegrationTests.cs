extern alias RulesApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class RulesApiIntegrationTests : IClassFixture<WebApplicationFactory<RulesApi::Program>>
{
    private readonly HttpClient _client;

    public RulesApiIntegrationTests(WebApplicationFactory<RulesApi::Program> factory)
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

        Assert.Equal("Qams.Rules.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateEvaluateAndTriggerRuleFlow_ReturnsExpectedResults()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            name = "CAPA Approval Rule",
            resourceType = "CapaCase",
            triggerEvent = "CapaClosed",
            action = "approve",
            description = "Require qa-manager to approve CAPA closure.",
            effect = "RequireRole:qa-manager",
            enabled = true
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/rules")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-rule-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var ruleId = createDoc.RootElement.GetProperty("data").GetProperty("ruleId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(ruleId));

        var evaluateRequest = new
        {
            tenantId = "tenant-demo",
            resourceType = "CapaCase",
            action = "approve",
            userRoles = Array.Empty<string>(),
            attributes = new Dictionary<string, string>()
        };

        var evaluateMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/rules/evaluate")
        {
            Content = JsonContent.Create(evaluateRequest)
        };
        evaluateMessage.Headers.Add("X-Tenant-Id", "tenant-demo");

        var evaluateResponse = await _client.SendAsync(evaluateMessage);
        await EnsureSuccessStatusCodeWithContent(evaluateResponse);

        using var evaluateDoc = await JsonDocument.ParseAsync(await evaluateResponse.Content.ReadAsStreamAsync());
        var evaluateRoot = evaluateDoc.RootElement.GetProperty("data");
        Assert.False(evaluateRoot.GetProperty("allowed").GetBoolean());
        Assert.Equal("Rules evaluation denied.", evaluateRoot.GetProperty("reason").GetString());

        var triggerRequest = new
        {
            tenantId = "tenant-demo",
            resourceType = "CapaCase",
            triggerEvent = "CapaClosed",
            payload = new Dictionary<string, string> { ["capaCaseId"] = "capa-123" }
        };

        var triggerMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/rules/trigger")
        {
            Content = JsonContent.Create(triggerRequest)
        };
        triggerMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        triggerMessage.Headers.Add("Idempotency-Key", "trigger-rule-001");

        var triggerResponse = await _client.SendAsync(triggerMessage);
        await EnsureSuccessStatusCodeWithContent(triggerResponse);

        using var triggerDoc = await JsonDocument.ParseAsync(await triggerResponse.Content.ReadAsStreamAsync());
        var triggerRoot = triggerDoc.RootElement.GetProperty("data");
        Assert.True(triggerRoot.GetProperty("triggered").GetBoolean());
        Assert.Equal("Triggered automation actions.", triggerRoot.GetProperty("reason").GetString());
    }
}
