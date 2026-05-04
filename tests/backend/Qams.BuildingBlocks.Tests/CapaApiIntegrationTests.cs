extern alias CapaApi;

using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Qams.BuildingBlocks.Tests;

public class CapaApiIntegrationTests : IClassFixture<WebApplicationFactory<CapaApi::Program>>
{
    private readonly HttpClient _client;

    public CapaApiIntegrationTests(WebApplicationFactory<CapaApi::Program> factory)
    {
        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var existingDescriptors = services.Where(sd => sd.ServiceType == typeof(IHttpClientFactory)).ToList();
                foreach (var descriptor in existingDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton<IHttpClientFactory>(new FakeHttpClientFactory());
            });
        });

        _client = testFactory.CreateClient();
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
    public async Task CreateAndCloseCapaCase_ReturnsClosedCapaCase()
    {
        var createRequest = new
        {
            tenantId = "tenant-demo",
            title = "CAPA closure workflow test",
            severity = "High",
            sourceQualityEventId = (string?)null
        };

        var createMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/capa-cases")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        createMessage.Headers.Add("Idempotency-Key", "create-capa-001");

        var createResponse = await _client.SendAsync(createMessage);
        await EnsureSuccessStatusCodeWithContent(createResponse);

        using var createDoc = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
        var capaCaseId = createDoc.RootElement.GetProperty("data").GetProperty("capaCaseId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(capaCaseId));

        var closeRequest = new
        {
            tenantId = "tenant-demo",
            effectivenessVerified = true,
            closureRationale = "Effectiveness verified by QA management."
        };

        var closeMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/capa-cases/{capaCaseId}/close")
        {
            Content = JsonContent.Create(closeRequest)
        };
        closeMessage.Headers.Add("X-Tenant-Id", "tenant-demo");
        closeMessage.Headers.Add("Idempotency-Key", "close-capa-001");

        var closeResponse = await _client.SendAsync(closeMessage);
        await EnsureSuccessStatusCodeWithContent(closeResponse);

        using var closeDoc = await JsonDocument.ParseAsync(await closeResponse.Content.ReadAsStreamAsync());
        var responseData = closeDoc.RootElement.GetProperty("data");
        Assert.Equal("Closed", responseData.GetProperty("status").GetString());
        Assert.Equal(capaCaseId, responseData.GetProperty("capaCaseId").GetString());
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new FakeEsignatureHandler())
            {
                BaseAddress = new Uri("http://localhost")
            };
        }
    }

    private sealed class FakeEsignatureHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri?.AbsolutePath == "/api/v1/esignatures")
            {
                var payload = new
                {
                    data = new
                    {
                        signatureId = "sig-001",
                        tenantId = "tenant-demo",
                        recordType = "CapaCase",
                        recordId = "capa-test",
                        recordVersion = "1",
                        signedMeaning = "Approved CAPA closure",
                        reason = "Effectiveness verified by QA management.",
                        signerUserId = "unknown",
                        signerDisplayName = "QA Manager",
                        reauthMethod = "mfa",
                        reauthReference = "auth-event-demo",
                        recordHash = "sha256:capa-test-v1-closed",
                        previousAuditHash = (string?)null
                    },
                    correlationId = "test-correlation"
                };

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(payload)
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
