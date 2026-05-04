extern alias PolicyApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class PolicyApiIntegrationTests : IClassFixture<WebApplicationFactory<PolicyApi::Program>>
{
    private readonly HttpClient _client;

    public PolicyApiIntegrationTests(WebApplicationFactory<PolicyApi::Program> factory)
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

        Assert.Equal("Qams.Policy.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task EvaluatePolicy_AllowsQAManagerForCapaApproval()
    {
        var request = new
        {
            resourceType = "CapaCase",
            action = "approve",
            userRoles = new[] { "qa-manager" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/policy/evaluate")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetProperty("allowed").GetBoolean());
        Assert.Equal("Access granted.", data.GetProperty("reason").GetString());
    }

    [Fact]
    public async Task EvaluatePolicy_DeniesUnauthorizedRoleForCapaApproval()
    {
        var request = new
        {
            resourceType = "CapaCase",
            action = "approve",
            userRoles = new[] { "technician" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/policy/evaluate")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.False(data.GetProperty("allowed").GetBoolean());
        Assert.Equal("Access denied.", data.GetProperty("reason").GetString());
    }

    [Fact]
    public async Task EvaluatePolicy_AllowsAuditorForAuditFindingCreation()
    {
        var request = new
        {
            resourceType = "AuditFinding",
            action = "create",
            userRoles = new[] { "auditor" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/policy/evaluate")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetProperty("allowed").GetBoolean());
        var requiredRoles = data.GetProperty("requiredRoles");
        var roles = requiredRoles.EnumerateArray().Select(r => r.GetString()).ToArray();
        Assert.Contains("auditor", roles);
    }

    [Fact]
    public async Task EvaluatePolicy_ReturnsNoRulesForUnknownResourceType()
    {
        var request = new
        {
            resourceType = "UnknownResource",
            action = "read",
            userRoles = new[] { "admin" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/policy/evaluate")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-life-sciences-demo");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.False(data.GetProperty("allowed").GetBoolean());
        Assert.Equal("No applicable policy rules found.", data.GetProperty("reason").GetString());
    }
}
