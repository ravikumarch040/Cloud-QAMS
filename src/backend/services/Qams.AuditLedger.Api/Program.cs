using Qams.BuildingBlocks.Audit;
using Qams.BuildingBlocks.Common;
using Qams.AuditLedger.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuditLedgerStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.AuditLedger.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/audit-ledger", (HttpRequest request, AuditLedgerStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var entries = store.List(context.TenantId);

    return Results.Ok(ApiResponse.Ok(entries, context.CorrelationId));
});

app.MapPost("/api/v1/audit-ledger", (AppendAuditLedgerEntryRequest command, HttpRequest request, AuditLedgerStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<AuditLedgerEntry>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<AuditLedgerEntry>(
            new ApiError("tenant_mismatch", "Request tenant header must match audit entry tenant."),
            context.CorrelationId));
    }

    var entry = store.Append(command, Guid.NewGuid(), context.CorrelationId);
    return Results.Created($"/api/v1/audit-ledger/{entry.AuditEntryId}", ApiResponse.Ok(entry, context.CorrelationId));
});

app.MapGet("/api/v1/audit-ledger/verify", (HttpRequest request, AuditLedgerStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<AuditLedgerVerificationResult>(headerError, context.CorrelationId));
    }

    var result = store.Verify(context.TenantId);
    return Results.Ok(ApiResponse.Ok(result, context.CorrelationId));
});

app.Run();

public partial class Program { }
