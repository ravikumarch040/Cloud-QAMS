using Qams.BuildingBlocks.Common;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var capaCases = new List<CapaCaseSummary>
{
    new("capa-0001", "tenant-life-sciences-demo", "Open", "Temperature excursion corrective action", "High", "Investigation", DateTimeOffset.UtcNow)
};

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Capa.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/capa-cases", (HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var casesForTenant = string.IsNullOrWhiteSpace(context.TenantId)
        ? capaCases
        : capaCases.Where(item => item.TenantId == context.TenantId).ToList();

    return Results.Ok(ApiResponse.Ok<IReadOnlyCollection<CapaCaseSummary>>(casesForTenant, context.CorrelationId));
});

app.MapPost("/api/v1/capa-cases", (CreateCapaCaseRequest command, HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<CapaCaseSummary>(headerError, context.CorrelationId));
    }

    var created = new CapaCaseSummary(
        $"capa-{Guid.NewGuid():N}",
        context.TenantId,
        "Open",
        command.Title,
        command.Severity,
        "Triage",
        DateTimeOffset.UtcNow);

    capaCases.Add(created);
    return Results.Created($"/api/v1/capa-cases/{created.CapaCaseId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/capa-cases/{capaCaseId}/close", (string capaCaseId, CloseCapaCaseRequest command, HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<CapaCaseSummary>(headerError, context.CorrelationId));
    }

    var existing = capaCases.FirstOrDefault(item => item.CapaCaseId == capaCaseId && item.TenantId == context.TenantId);
    if (existing is null)
    {
        return Results.NotFound(ApiResponse.Fail<CapaCaseSummary>(new ApiError("capa_not_found", $"CAPA case '{capaCaseId}' was not found."), context.CorrelationId));
    }

    if (!command.EffectivenessVerified || !command.HasRequiredESignature)
    {
        return Results.BadRequest(ApiResponse.Fail<CapaCaseSummary>(
            new ApiError("closure_controls_not_satisfied", "CAPA closure requires effectiveness verification and required e-signature."),
            context.CorrelationId));
    }

    var closed = existing with { Status = "Closed", CurrentWorkflowStage = "Closed" };
    capaCases.Remove(existing);
    capaCases.Add(closed);
    return Results.Ok(ApiResponse.Ok(closed, context.CorrelationId));
});

app.Run();

sealed record CapaCaseSummary(
    string CapaCaseId,
    string TenantId,
    string Status,
    string Title,
    string Severity,
    string CurrentWorkflowStage,
    DateTimeOffset CreatedAtUtc);

sealed record CreateCapaCaseRequest(string Title, string Severity, string? SourceQualityEventId);
sealed record CloseCapaCaseRequest(bool EffectivenessVerified, bool HasRequiredESignature, string ClosureRationale);
