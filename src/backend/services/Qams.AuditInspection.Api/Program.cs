using System.Text.Json.Serialization;
using Qams.BuildingBlocks.Common;
using Qams.AuditInspection.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuditInspectionStore>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.AuditInspection.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/inspections", (HttpRequest request, AuditInspectionStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var inspections = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(inspections, context.CorrelationId));
});

app.MapGet("/api/v1/inspections/{inspectionId}", (string inspectionId, HttpRequest request, AuditInspectionStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var inspection = store.Get(inspectionId, context.TenantId);
    if (inspection is null)
    {
        return Results.NotFound(ApiResponse.Fail<InspectionRecord>(new ApiError("inspection_not_found", "Inspection not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(inspection, context.CorrelationId));
});

app.MapPost("/api/v1/inspections", (CreateInspectionRequest command, HttpRequest request, AuditInspectionStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<InspectionRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<InspectionRecord>(new ApiError("tenant_mismatch", "Request tenant header must match inspection tenant."), context.CorrelationId));
    }

    var created = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/inspections/{created.InspectionId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/inspections/{inspectionId}/complete", (string inspectionId, CompleteInspectionRequest command, HttpRequest request, AuditInspectionStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<InspectionRecord>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<InspectionRecord>(new ApiError("tenant_mismatch", "Request tenant header must match inspection tenant."), context.CorrelationId));
    }

    var completed = store.Complete(inspectionId, command, context.TenantId, context.CorrelationId);
    if (completed is null)
    {
        return Results.NotFound(ApiResponse.Fail<InspectionRecord>(new ApiError("inspection_not_found", "Inspection not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(completed, context.CorrelationId));
});

app.Run();

public partial class Program { }
