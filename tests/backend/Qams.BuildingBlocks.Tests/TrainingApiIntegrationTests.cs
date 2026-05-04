extern alias TrainingApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class TrainingApiIntegrationTests : IClassFixture<WebApplicationFactory<TrainingApi::Program>>
{
    private readonly HttpClient _client;

    public TrainingApiIntegrationTests(WebApplicationFactory<TrainingApi::Program> factory)
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

        Assert.Equal("Qams.Training.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndEnrollTrainingCourse_ReturnsEnrollment()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            title = "GMP Basics",
            description = "Good Manufacturing Practices training",
            durationHours = 8
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/training-courses")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-course-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var courseId = createDoc.RootElement.GetProperty("data").GetProperty("courseId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(courseId));

        var enrollRequest = new
        {
            tenantId = "tenant-demo",
            userId = "user-123",
            enrollmentDate = "2026-05-04"
        };

        var enrollMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/training-courses/{courseId}/enroll")
        {
            Content = JsonContent.Create(enrollRequest)
        };
        enrollMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        enrollMessage.Headers.Add("Idempotency-Key", "enroll-001");

        var enrollResponse = await _client.SendAsync(enrollMessage);
        await EnsureSuccessStatusCodeWithContent(enrollResponse);

        using var enrollDoc = await JsonDocument.ParseAsync(await enrollResponse.Content.ReadAsStreamAsync());
        var enrollmentId = enrollDoc.RootElement.GetProperty("data").GetProperty("enrollmentId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(enrollmentId));
    }
}
