extern alias WorkflowApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class WorkflowApiIntegrationTests : IClassFixture<WebApplicationFactory<WorkflowApi::Program>>
{
    private readonly HttpClient _client;

    public WorkflowApiIntegrationTests(WebApplicationFactory<WorkflowApi::Program> factory)
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

        Assert.Equal("Qams.Workflow.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ListWorkflowDefinitions_ReturnsSeedWorkflows()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/workflow-definitions");
        request.Headers.Add("X-Correlation-Id", "test-corr-001");

        var response = await _client.SendAsync(request);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() > 0);

        var firstWorkflow = data[0];
        Assert.NotNull(firstWorkflow.GetProperty("workflowDefinitionId").GetString());
        Assert.Equal("CAPA Approval And Closure", firstWorkflow.GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateWorkflowDefinition_ReturnsCreatedWorkflow()
    {
        var createRequest = new
        {
            recordType = "Complaint",
            name = "Complaint Handling Workflow",
            stages = new[]
            {
                new
                {
                    stageId = "receipt",
                    stageName = "Receipt & Acknowledgment",
                    sequenceNumber = 1,
                    requiresApproval = false,
                    targetDurationDays = 1
                },
                new
                {
                    stageId = "investigation",
                    stageName = "Investigation",
                    sequenceNumber = 2,
                    requiresApproval = true,
                    targetDurationDays = 14
                }
            }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/workflow-definitions")
        {
            Content = JsonContent.Create(createRequest)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-workflow-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");
        Assert.NotNull(data.GetProperty("workflowDefinitionId").GetString());
        Assert.Equal("Complaint Handling Workflow", data.GetProperty("name").GetString());
        Assert.Equal("0.1.0-draft", data.GetProperty("version").GetString());
    }

    [Fact]
    public async Task CreateWorkflowDefinition_RequiresIdempotencyKey()
    {
        var createRequest = new
        {
            recordType = "Document",
            name = "Document Control Workflow",
            stages = new[]
            {
                new
                {
                    stageId = "draft",
                    stageName = "Draft",
                    sequenceNumber = 1,
                    requiresApproval = false,
                    targetDurationDays = 7
                }
            }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/workflow-definitions")
        {
            Content = JsonContent.Create(createRequest)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        // Missing Idempotency-Key

        var response = await _client.SendAsync(message);
        Assert.False(response.IsSuccessStatusCode);
    }
}
