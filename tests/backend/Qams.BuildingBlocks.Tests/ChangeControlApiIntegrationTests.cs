extern alias ChangeControlApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ChangeControlApiIntegrationTests : IClassFixture<WebApplicationFactory<ChangeControlApi::Program>>
{
    private readonly HttpClient _client;

    public ChangeControlApiIntegrationTests(WebApplicationFactory<ChangeControlApi::Program> factory)
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

        Assert.Equal("Qams.ChangeControl.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndApproveChangeRequest_ReturnsApprovedRequest()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            title = "Update manufacturing process",
            description = "Modify assembly line to improve quality control",
            impactAnalysis = "Minor impact on production schedule"
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/change-requests")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-change-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var changeRequestId = createDoc.RootElement.GetProperty("data").GetProperty("changeRequestId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(changeRequestId));

        var approveRequest = new
        {
            tenantId = "tenant-demo",
            approvedByUserId = "qa-manager",
            approvalComments = "Approved with minor modifications required"
        };

        var approveMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/change-requests/{changeRequestId}/approve")
        {
            Content = JsonContent.Create(approveRequest)
        };
        approveMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        approveMessage.Headers.Add("Idempotency-Key", "approve-change-001");

        var approveResponse = await _client.SendAsync(approveMessage);
        await EnsureSuccessStatusCodeWithContent(approveResponse);

        using var approveDoc = await JsonDocument.ParseAsync(await approveResponse.Content.ReadAsStreamAsync());
        var responseData = approveDoc.RootElement.GetProperty("data");
        Assert.Equal("Approved", responseData.GetProperty("status").GetString());
    }
}