extern alias ProductMasterApi;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Qams.BuildingBlocks.Tests;

public class ProductMasterApiIntegrationTests : IClassFixture<WebApplicationFactory<ProductMasterApi::Program>>
{
    private readonly HttpClient _client;

    public ProductMasterApiIntegrationTests(WebApplicationFactory<ProductMasterApi::Program> factory)
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

        Assert.Equal("Qams.ProductMaster.Api", root.GetProperty("service").GetString());
        Assert.Equal("Healthy", root.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        var request = new
        {
            tenantId = "tenant-demo",
            productName = "Medical Device Implant",
            productCode = "MDI-001",
            productType = "ImplantableDevice",
            description = "Cardiac pacemaker device",
            regulatoryClass = "Class III",
            manufacturingSite = "Site-A",
            specifications = new { dimensions = "10x5x2cm", weight = "150g" }
        };

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/products")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Tenant-Id", "tenant-demo");
        message.Headers.Add("Idempotency-Key", "create-product-001");

        var response = await _client.SendAsync(message);
        await EnsureSuccessStatusCodeWithContent(response);

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = doc.RootElement.GetProperty("data");

        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("productId").GetString()));
        Assert.Equal("tenant-demo", data.GetProperty("tenantId").GetString());
        Assert.Equal("Medical Device Implant", data.GetProperty("name").GetString());
        Assert.Equal("MDI-001", data.GetProperty("productCode").GetString());
    }
}