using Qams.BuildingBlocks.Common;
using Qams.Integration.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IntegrationStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Integration.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/connectors", (HttpRequest request, IntegrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var connectors = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(connectors, context.CorrelationId));
});

app.MapPost("/api/v1/connectors", (CreateConnectorRequest command, HttpRequest request, IntegrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ConnectorDefinition>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ConnectorDefinition>(new ApiError("tenant_mismatch", "Request tenant header must match tenant."), context.CorrelationId));
    }

    var connector = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/connectors/{connector.ConnectorId}", ApiResponse.Ok(connector, context.CorrelationId));
});

app.MapPost("/api/v1/connectors/{connectorId}/test", (string connectorId, TestConnectorRequest command, HttpRequest request, IntegrationStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ConnectorTestResult>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ConnectorTestResult>(new ApiError("tenant_mismatch", "Request tenant header must match tenant."), context.CorrelationId));
    }

    var testResult = store.Test(connectorId, command, context.TenantId, context.CorrelationId);
    return Results.Ok(ApiResponse.Ok(testResult, context.CorrelationId));
});

app.Run();

public partial class Program { }
