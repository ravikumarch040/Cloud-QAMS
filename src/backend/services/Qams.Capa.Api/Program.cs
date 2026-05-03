using Qams.BuildingBlocks.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

var capaCases = new List<CapaCaseSummary>
{
    new("capa-0001", "tenant-life-sciences-demo", "Open", "Temperature excursion corrective action", "High", "Investigation", DateTimeOffset.UtcNow, "1")
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
        DateTimeOffset.UtcNow,
        "1");

    capaCases.Add(created);
    return Results.Created($"/api/v1/capa-cases/{created.CapaCaseId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.MapPost("/api/v1/capa-cases/{capaCaseId}/close", async (string capaCaseId, CloseCapaCaseRequest command, HttpRequest request, IHttpClientFactory httpClientFactory) =>
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

    if (!command.EffectivenessVerified)
    {
        return Results.BadRequest(ApiResponse.Fail<CapaCaseSummary>(
            new ApiError("closure_controls_not_satisfied", "CAPA closure requires effectiveness verification."),
            context.CorrelationId));
    }

    // Create e-signature for closure
    var recordHash = $"sha256:capa-{existing.CapaCaseId}-v{existing.Version ?? "1"}-closed"; // Simplified hash
    var esignatureRequest = new
    {
        tenantId = context.TenantId,
        recordType = "CapaCase",
        recordId = existing.CapaCaseId,
        recordVersion = existing.Version ?? "1",
        signedMeaning = "Approved CAPA closure",
        reason = command.ClosureRationale,
        signerUserId = context.ActorId ?? "unknown",
        signerDisplayName = "QA Manager", // TODO: Get from identity
        reauthMethod = "mfa", // TODO: Get from auth context
        reauthReference = "auth-event-demo", // TODO: Get from auth
        recordHash = recordHash,
        previousAuditHash = (string?)null
    };

    var httpClient = httpClientFactory.CreateClient();
    httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", context.TenantId);
    httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", context.CorrelationId);
    httpClient.DefaultRequestHeaders.Add("Idempotency-Key", request.Headers["Idempotency-Key"].ToString());
    httpClient.DefaultRequestHeaders.Add("X-Actor-Context", request.Headers["X-Actor-Context"].ToString());

    var esignatureResponse = await httpClient.PostAsJsonAsync("http://localhost:5203/api/v1/esignatures", esignatureRequest);
    if (!esignatureResponse.IsSuccessStatusCode)
    {
        return Results.BadRequest(ApiResponse.Fail<CapaCaseSummary>(
            new ApiError("esignature_failed", "Failed to create required e-signature for CAPA closure."),
            context.CorrelationId));
    }

    var closed = existing with { Status = "Closed", CurrentWorkflowStage = "Closed", Version = (existing.Version ?? "1") + "-closed" };
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
    DateTimeOffset CreatedAtUtc,
    string? Version = null);

sealed record CreateCapaCaseRequest(string Title, string Severity, string? SourceQualityEventId);
sealed record CloseCapaCaseRequest(bool EffectivenessVerified, string ClosureRationale);
