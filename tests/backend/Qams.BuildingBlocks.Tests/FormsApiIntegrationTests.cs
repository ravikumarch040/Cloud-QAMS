extern alias FormsApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class FormsApiIntegrationTests : IClassFixture<WebApplicationFactory<FormsApi::Program>>
{
    private readonly HttpClient _client;

    public FormsApiIntegrationTests(WebApplicationFactory<FormsApi::Program> factory)
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

        Assert.Equal("Qams.Forms.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndValidateFormDefinition_ReturnsSuccess()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            name = "CAPA Closure Checklist",
            version = "1.0.0",
            status = "Draft",
            fields = new[]
            {
                new { fieldId = "root-cause", label = "Root Cause", fieldType = 0, required = true },
                new { fieldId = "completion-date", label = "Completion Date", fieldType = 2, required = true }
            }
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/form-definitions")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-form-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var formDefinitionId = createDoc.RootElement.GetProperty("data").GetProperty("formDefinitionId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(formDefinitionId));

        var validateRequest = new
        {
            tenantId = "tenant-demo",
            formDefinitionId,
            values = new Dictionary<string, object?>
            {
                ["root-cause"] = "Procedure deviation",
                ["completion-date"] = "2026-05-10"
            }
        };

        var validateMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/form-definitions/{formDefinitionId}/validate")
        {
            Content = JsonContent.Create(validateRequest)
        };
        validateMessage.Headers.Add("X-Tenant-Id", "tenant-demo");

        var validateResponse = await _client.SendAsync(validateMessage);
        await EnsureSuccessStatusCodeWithContent(validateResponse);

        using var validationDoc = await JsonDocument.ParseAsync(await validateResponse.Content.ReadAsStreamAsync());
        var isValid = validationDoc.RootElement.GetProperty("data").GetProperty("isValid").GetBoolean();
        Assert.True(isValid);
    }
}
