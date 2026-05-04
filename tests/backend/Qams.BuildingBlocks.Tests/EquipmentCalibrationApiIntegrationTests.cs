extern alias EquipmentCalibrationApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class EquipmentCalibrationApiIntegrationTests : IClassFixture<WebApplicationFactory<EquipmentCalibrationApi::Program>>
{
    private readonly HttpClient _client;

    public EquipmentCalibrationApiIntegrationTests(WebApplicationFactory<EquipmentCalibrationApi::Program> factory)
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

        Assert.Equal("Qams.EquipmentCalibration.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateAndCalibrateEquipment_ReturnsCalibratedEquipment()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            equipmentName = "Digital Scale Model X1",
            equipmentType = "Measurement",
            manufacturer = "Precision Instruments",
            model = "X1-2024",
            serialNumber = "SN-12345",
            calibrationIntervalMonths = 12
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/equipment-calibrations")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-equipment-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var equipmentId = createDoc.RootElement.GetProperty("data").GetProperty("equipmentId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(equipmentId));

        var calibrateRequest = new
        {
            tenantId = "tenant-demo",
            calibrationAtUtc = DateTimeOffset.Parse("2026-05-04T00:00:00Z"),
            intervalDays = 365,
            outOfTolerance = false
        };

        var calibrateMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/equipment-calibrations/{equipmentId}/calibrate")
        {
            Content = JsonContent.Create(calibrateRequest)
        };
        calibrateMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        calibrateMessage.Headers.Add("Idempotency-Key", "calibrate-equipment-001");

        var calibrateResponse = await _client.SendAsync(calibrateMessage);
        await EnsureSuccessStatusCodeWithContent(calibrateResponse);

        using var calibrateDoc = await JsonDocument.ParseAsync(await calibrateResponse.Content.ReadAsStreamAsync());
        var responseData = calibrateDoc.RootElement.GetProperty("data");
        Assert.Equal("Scheduled", responseData.GetProperty("status").GetString());
    }
}