using Qams.BuildingBlocks.Common;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var qualityEvents = new List<QualityEventSummary>
{
    new("qe-0001", "tenant-life-sciences-demo", "Deviation", "Open", "Temperature excursion in controlled storage", "High", DateTimeOffset.UtcNow)
};

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.QualityEvents.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/quality-events", (HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var eventsForTenant = string.IsNullOrWhiteSpace(context.TenantId)
        ? qualityEvents
        : qualityEvents.Where(item => item.TenantId == context.TenantId).ToList();

    return Results.Ok(ApiResponse.Ok<IReadOnlyCollection<QualityEventSummary>>(eventsForTenant, context.CorrelationId));
});

app.MapPost("/api/v1/quality-events", (CreateQualityEventRequest command, HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<QualityEventSummary>(headerError, context.CorrelationId));
    }

    var created = new QualityEventSummary(
        $"qe-{Guid.NewGuid():N}",
        context.TenantId,
        command.EventType,
        "Open",
        command.Title,
        command.Severity,
        DateTimeOffset.UtcNow);

    qualityEvents.Add(created);
    return Results.Created($"/api/v1/quality-events/{created.QualityEventId}", ApiResponse.Ok(created, context.CorrelationId));
});

app.Run();

sealed record QualityEventSummary(
    string QualityEventId,
    string TenantId,
    string EventType,
    string Status,
    string Title,
    string Severity,
    DateTimeOffset CreatedAtUtc);

sealed record CreateQualityEventRequest(string EventType, string Title, string Severity);
