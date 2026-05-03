using Qams.BuildingBlocks.Common;
using Qams.ValidationEvidence.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ValidationEvidenceStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.ValidationEvidence.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/validation-evidence", (HttpRequest request, ValidationEvidenceStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var evidence = store.List(context.TenantId);
    return Results.Ok(ApiResponse.Ok(evidence, context.CorrelationId));
});

app.MapPost("/api/v1/validation-evidence", (CreateValidationEvidenceRequest command, HttpRequest request, ValidationEvidenceStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ValidationEvidencePack>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<ValidationEvidencePack>(new ApiError("tenant_mismatch", "Request tenant header must match validation evidence tenant."), context.CorrelationId));
    }

    var evidence = store.Create(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/validation-evidence/{evidence.EvidencePackId}", ApiResponse.Ok(evidence, context.CorrelationId));
});

app.MapPost("/api/v1/validation-evidence/{evidencePackId}/approve", (string evidencePackId, ApproveValidationEvidenceRequest command, HttpRequest request, ValidationEvidenceStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<ValidationEvidencePack>(headerError, context.CorrelationId));
    }

    var approved = store.Approve(evidencePackId, command, context.TenantId, context.CorrelationId);
    if (approved is null)
    {
        return Results.NotFound(ApiResponse.Fail<ValidationEvidencePack>(new ApiError("evidence_not_found", "Validation evidence pack not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(approved, context.CorrelationId));
});

app.Run();
