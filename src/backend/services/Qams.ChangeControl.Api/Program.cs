using Qams.BuildingBlocks.Common;
using Qams.ChangeControl.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ChangeControlStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.ChangeControl.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/change-requests", (HttpRequest request, ChangeControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var changes = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(changes, context.CorrelationId));
});

app.MapGet("/api/v1/change-requests/{changeRequestId}", (string changeRequestId, HttpRequest request, ChangeControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var change = store.Get(changeRequestId, context.TenantId);
    if (change is null)
    {
        return Results.NotFound(ApiResponse.Fail<ChangeRequestRecord>(new ApiError("change_request_not_found", "Change request not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(change, context.CorrelationId));
});

app.MapPost("/api/v1/change-requests", (CreateChangeRequestRequest command, HttpRequest request, ChangeControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ChangeRequestRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ChangeRequestRecord>(new ApiError("tenant_mismatch", "Request tenant header must match change request tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/change-requests/{created.ChangeRequestId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/change-requests/{changeRequestId}/approve", (string changeRequestId, ApproveChangeRequestRequest command, HttpRequest request, ChangeControlStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ChangeRequestRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ChangeRequestRecord>(new ApiError("tenant_mismatch", "Request tenant header must match change request tenant."), context.CorrelationId));
    }

    var approved = store.Approve(changeRequestId, command, context.TenantId, context.CorrelationId);
    if (approved is null)
    {
        return Results.NotFound(ApiResponse.Fail<ChangeRequestRecord>(new ApiError("change_request_not_found", "Change request not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(approved, context.CorrelationId));
});

app.Run();
