extern alias NotificationApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class NotificationApiIntegrationTests : IClassFixture<WebApplicationFactory<NotificationApi::Program>>
{
    private readonly HttpClient _client;

    public NotificationApiIntegrationTests(WebApplicationFactory<NotificationApi::Program> factory)
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

        Assert.Equal("Qams.Notification.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndAcknowledgeNotification_ReturnsAcknowledgedNotification()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            recipientUserId = "user-123",
            subject = "Quality Issue Detected",
            message = "A new quality issue has been reported in the system"
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/notifications")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-notification-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var notificationId = createDoc.RootElement.GetProperty("data").GetProperty("notificationId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(notificationId));

        var acknowledgeMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/notifications/{notificationId}/acknowledge");
        acknowledgeMessage.Headers.Add("X-Tenant-Id", "tenant-demo");

        var acknowledgeResponse = await _client.SendAsync(acknowledgeMessage);
        await EnsureSuccessStatusCodeWithContent(acknowledgeResponse);

        using var acknowledgeDoc = await JsonDocument.ParseAsync(await acknowledgeResponse.Content.ReadAsStreamAsync());
        var responseData = acknowledgeDoc.RootElement.GetProperty("data");
        Assert.Equal("Acknowledged", responseData.GetProperty("status").GetString());
    }
}